using System.Buffers.Binary;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Reads image dimensions from supported image file headers.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ImageDimensionsReader
{
    private const int _maximumHeaderBytes = 1024 * 1024;

    private static readonly byte[] _pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    private static readonly byte[] _standaloneJpegMarkers = [0xD8, 0xD9, 0x01];

    private static readonly byte[] _startOfFrameJpegMarkers =
    [
        0xC0, 0xC1, 0xC2, 0xC3,
        0xC5, 0xC6, 0xC7,
        0xC9, 0xCA, 0xCB,
        0xCD, 0xCE, 0xCF
    ];

    /// <summary>
    /// Attempts to read image dimensions from an uploaded file.
    /// </summary>
    ///
    /// <param name="file">The uploaded image file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The image dimensions when they can be read; otherwise, <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static async Task<ImageDimensions?> TryReadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length <= 0)
            return null;

        byte[] header = await ReadHeaderAsync(file, cancellationToken);

        return TryRead(header);
    }

    /// <summary>
    /// Reads the file header bytes used for image dimension detection.
    /// </summary>
    ///
    /// <param name="file">The uploaded image file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The read header bytes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task<byte[]> ReadHeaderAsync(IFormFile file, CancellationToken cancellationToken)
    {
        int length = (int)Math.Min(file.Length, _maximumHeaderBytes);
        byte[] buffer = new byte[length];

        await using Stream stream = file.OpenReadStream();
        int offset = 0;

        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);

            if (read == 0)
                break;

            offset += read;
        }

        return offset == buffer.Length ? buffer : buffer[..offset];
    }

    /// <summary>
    /// Attempts to read image dimensions from known image file headers.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    ///
    /// <returns>The image dimensions when a supported header is found; otherwise, <c>null</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static ImageDimensions? TryRead(ReadOnlySpan<byte> header)
        => TryReadPng(header, out ImageDimensions dimensions)
           || TryReadGif(header, out dimensions)
           || TryReadJpeg(header, out dimensions)
           || TryReadWebP(header, out dimensions) ? dimensions : null;

    /// <summary>
    /// Attempts to read image dimensions from a PNG header.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when PNG dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadPng(ReadOnlySpan<byte> header, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasPngHeader(header))
            return false;

        int width = BinaryPrimitives.ReadInt32BigEndian(header.Slice(16, 4));
        int height = BinaryPrimitives.ReadInt32BigEndian(header.Slice(20, 4));

        return TryCreateDimensions(width, height, out dimensions);
    }

    /// <summary>
    /// Attempts to read image dimensions from a GIF header.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when GIF dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadGif(ReadOnlySpan<byte> header, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasGifHeader(header))
            return false;

        int width = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(6, 2));
        int height = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(8, 2));

        return TryCreateDimensions(width, height, out dimensions);
    }

    /// <summary>
    /// Attempts to read image dimensions from a JPEG header.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when JPEG dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadJpeg(ReadOnlySpan<byte> header, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasJpegHeader(header))
            return false;

        int offset = 2;

        while (offset + 3 < header.Length)
        {
            if (header[offset] != 0xFF)
                return false;

            while (offset < header.Length && header[offset] == 0xFF)
                offset++;

            if (offset >= header.Length)
                return false;

            byte marker = header[offset++];

            if (IsStandaloneJpegMarker(marker))
                continue;

            if (offset + 1 >= header.Length)
                return false;

            int segmentLength = BinaryPrimitives.ReadUInt16BigEndian(header.Slice(offset, 2));

            if (!HasValidSegmentLength(header, offset, segmentLength))
                return false;

            if (IsStartOfFrameMarker(marker))
            {
                if (segmentLength < 7)
                    return false;

                int height = BinaryPrimitives.ReadUInt16BigEndian(header.Slice(offset + 3, 2));
                int width = BinaryPrimitives.ReadUInt16BigEndian(header.Slice(offset + 5, 2));

                return TryCreateDimensions(width, height, out dimensions);
            }

            offset += segmentLength;
        }

        return false;
    }

    /// <summary>
    /// Attempts to read image dimensions from a WebP header.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when WebP dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadWebP(ReadOnlySpan<byte> header, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasWebPHeader(header))
            return false;

        int offset = 12;

        while (offset + 8 <= header.Length)
        {
            ReadOnlySpan<byte> chunkType = header.Slice(offset, 4);
            int chunkSize = BinaryPrimitives.ReadInt32LittleEndian(header.Slice(offset + 4, 4));

            int dataOffset = offset + 8;

            if (!HasValidChunkSize(header, dataOffset, chunkSize))
                return false;

            ReadOnlySpan<byte> chunk = header.Slice(dataOffset, chunkSize);

            if (TryReadWebPChunk(chunkType, chunk, out dimensions))
                return true;

            offset = dataOffset + chunkSize + chunkSize % 2;
        }

        return false;
    }

    /// <summary>
    /// Attempts to read dimensions from a VP8X WebP chunk.
    /// </summary>
    ///
    /// <param name="chunk">The WebP chunk bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when VP8X dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadVp8X(ReadOnlySpan<byte> chunk, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (chunk.Length < 10)
            return false;

        int width = 1 + ReadUInt24LittleEndian(chunk[4..7]);
        int height = 1 + ReadUInt24LittleEndian(chunk[7..10]);

        return TryCreateDimensions(width, height, out dimensions);
    }

    /// <summary>
    /// Attempts to read dimensions from a VP8L WebP chunk.
    /// </summary>
    ///
    /// <param name="chunk">The WebP chunk bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when VP8L dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadVp8L(ReadOnlySpan<byte> chunk, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasVp8LHeader(chunk))
            return false;

        int width = 1 + (((chunk[2] & 0x3F) << 8) | chunk[1]);
        int height = 1 + (((chunk[4] & 0x0F) << 10) | (chunk[3] << 2) | ((chunk[2] & 0xC0) >> 6));

        return TryCreateDimensions(width, height, out dimensions);
    }

    /// <summary>
    /// Attempts to read dimensions from a VP8 WebP chunk.
    /// </summary>
    ///
    /// <param name="chunk">The WebP chunk bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when VP8 dimensions were read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadVp8(ReadOnlySpan<byte> chunk, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (!HasVp8Header(chunk))
            return false;

        int width = BinaryPrimitives.ReadUInt16LittleEndian(chunk.Slice(6, 2)) & 0x3FFF;
        int height = BinaryPrimitives.ReadUInt16LittleEndian(chunk.Slice(8, 2)) & 0x3FFF;

        return TryCreateDimensions(width, height, out dimensions);
    }

    /// <summary>
    /// Creates image dimensions when width and height are valid.
    /// </summary>
    ///
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <param name="dimensions">The created image dimensions.</param>
    ///
    /// <returns><c>true</c> when the dimensions are valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryCreateDimensions(int width, int height, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(width, height);

        if (width <= 0)
            return false;

        return height > 0;
    }

    /// <summary>
    /// Checks whether header bytes contain a PNG signature and IHDR chunk marker.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    ///
    /// <returns><c>true</c> when the header is PNG; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasPngHeader(ReadOnlySpan<byte> header)
    {
        if (header.Length < 24)
            return false;

        return header[..8].SequenceEqual(_pngSignature) && header.Slice(12, 4).SequenceEqual("IHDR"u8);
    }

    /// <summary>
    /// Checks whether header bytes contain a GIF signature.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    ///
    /// <returns><c>true</c> when the header is GIF; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasGifHeader(ReadOnlySpan<byte> header)
    {
        if (header.Length < 10)
            return false;

        return header[..6].SequenceEqual("GIF87a"u8) || header[..6].SequenceEqual("GIF89a"u8);
    }

    /// <summary>
    /// Checks whether header bytes contain a JPEG start marker.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    ///
    /// <returns><c>true</c> when the header is JPEG; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasJpegHeader(ReadOnlySpan<byte> header)
    {
        if (header.Length < 4)
            return false;

        if (header[0] != 0xFF)
            return false;

        return header[1] == 0xD8;
    }

    /// <summary>
    /// Checks whether a JPEG marker does not contain segment payload bytes.
    /// </summary>
    ///
    /// <param name="marker">The JPEG marker.</param>
    ///
    /// <returns><c>true</c> when the marker is standalone; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsStandaloneJpegMarker(byte marker)
    {
        if (_standaloneJpegMarkers.Contains(marker))
            return true;

        return marker is >= 0xD0 and <= 0xD7;
    }

    /// <summary>
    /// Checks whether a JPEG segment length fits within the available header bytes.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="offset">The segment offset.</param>
    /// <param name="segmentLength">The segment length.</param>
    ///
    /// <returns><c>true</c> when the segment length is valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasValidSegmentLength(ReadOnlySpan<byte> header, int offset, int segmentLength)
    {
        if (segmentLength < 2)
            return false;

        return offset + segmentLength <= header.Length;
    }

    /// <summary>
    /// Checks whether header bytes contain a WebP RIFF signature.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    ///
    /// <returns><c>true</c> when the header is WebP; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasWebPHeader(ReadOnlySpan<byte> header)
    {
        if (header.Length < 30)
            return false;

        return header[..4].SequenceEqual("RIFF"u8) && header.Slice(8, 4).SequenceEqual("WEBP"u8);
    }

    /// <summary>
    /// Checks whether a WebP chunk size fits within the available header bytes.
    /// </summary>
    ///
    /// <param name="header">The image header bytes.</param>
    /// <param name="dataOffset">The chunk data offset.</param>
    /// <param name="chunkSize">The chunk size.</param>
    ///
    /// <returns><c>true</c> when the chunk size is valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasValidChunkSize(ReadOnlySpan<byte> header, int dataOffset, int chunkSize)
    {
        if (chunkSize < 0)
            return false;

        return dataOffset + chunkSize <= header.Length;
    }

    /// <summary>
    /// Attempts to read dimensions from a known WebP chunk type.
    /// </summary>
    ///
    /// <param name="chunkType">The WebP chunk type.</param>
    /// <param name="chunk">The WebP chunk bytes.</param>
    /// <param name="dimensions">The resolved image dimensions.</param>
    ///
    /// <returns><c>true</c> when a supported chunk type was read; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryReadWebPChunk(ReadOnlySpan<byte> chunkType, ReadOnlySpan<byte> chunk, out ImageDimensions dimensions)
    {
        dimensions = new ImageDimensions(0, 0);

        if (chunkType.SequenceEqual("VP8X"u8))
            return TryReadVp8X(chunk, out dimensions);

        if (chunkType.SequenceEqual("VP8L"u8))
            return TryReadVp8L(chunk, out dimensions);

        return chunkType.SequenceEqual("VP8 "u8) && TryReadVp8(chunk, out dimensions);
    }

    /// <summary>
    /// Checks whether a WebP chunk contains a VP8L signature.
    /// </summary>
    ///
    /// <param name="chunk">The WebP chunk bytes.</param>
    ///
    /// <returns><c>true</c> when the chunk is VP8L; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasVp8LHeader(ReadOnlySpan<byte> chunk)
    {
        if (chunk.Length < 5)
            return false;

        return chunk[0] == 0x2F;
    }

    /// <summary>
    /// Checks whether a WebP chunk contains a VP8 signature.
    /// </summary>
    ///
    /// <param name="chunk">The WebP chunk bytes.</param>
    ///
    /// <returns><c>true</c> when the chunk is VP8; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasVp8Header(ReadOnlySpan<byte> chunk)
    {
        if (chunk.Length < 10)
            return false;

        if (chunk[3] != 0x9D)
            return false;

        if (chunk[4] != 0x01)
            return false;

        return chunk[5] == 0x2A;
    }

    /// <summary>
    /// Reads a 24-bit little-endian integer from bytes.
    /// </summary>
    ///
    /// <param name="value">The value bytes.</param>
    ///
    /// <returns>The integer value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static int ReadUInt24LittleEndian(ReadOnlySpan<byte> value)
        => value[0] | (value[1] << 8) | (value[2] << 16);

    /// <summary>
    /// Checks whether a JPEG marker is a start-of-frame marker containing dimensions.
    /// </summary>
    ///
    /// <param name="marker">The JPEG marker.</param>
    ///
    /// <returns><c>true</c> when the marker contains dimensions; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsStartOfFrameMarker(byte marker) => _startOfFrameJpegMarkers.Contains(marker);
}
