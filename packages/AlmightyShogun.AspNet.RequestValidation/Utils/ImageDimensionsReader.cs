using System.Buffers.Binary;
using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Reads the pixel dimensions of an uploaded image from its header alone, without decoding the picture. Only the four formats a browser
/// will actually post from a file input are understood; anything else reports no dimensions rather than failing, so a dimension rule
/// declines an unrecognized format instead of rejecting the upload outright.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ImageDimensionsReader
{
    /// <summary>
    /// The most of a file this reader will pull into memory. A header needs far less, but a JPEG's frame marker can sit behind a large
    /// metadata segment, so the cap bounds a hostile file rather than the format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int MaximumHeaderBytes = 1024 * 1024;

    /// <summary>
    /// The eight bytes every PNG opens with. The non-ASCII first byte and the newline pair exist so a transfer that mangles line endings or
    /// strips the high bit corrupts the signature visibly rather than silently.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    /// The JPEG markers that carry no length word, so the walk steps over them by two bytes instead of reading a segment size that is not
    /// there.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] StandaloneJpegMarkers = [0xD8, 0xD9, 0x01];

    /// <summary>
    /// The start-of-frame markers whose segment carries the dimensions. The gaps in the run are the markers that share the numbering but
    /// mean something else, so the set is listed rather than expressed as a range.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] StartOfFrameJpegMarkers =
    [
        0xC0, 0xC1, 0xC2, 0xC3,
        0xC5, 0xC6, 0xC7,
        0xC9, 0xCA, 0xCB,
        0xCD, 0xCE, 0xCF
    ];

    /// <summary>
    /// Reads the dimensions of an uploaded file from its header, taking the first megabyte of it, or the whole file when it is smaller.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. A zero-length file is refused before any stream is opened.</param>
    /// <param name="cancellationToken">The token cancelling the header read.</param>
    ///
    /// <returns>
    /// The dimensions, or <c>null</c> when the file is empty, its format is not one of the four understood, or its header is truncated.
    /// </returns>
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
    /// Reads the leading bytes of an upload, capped so a hostile file cannot pull its whole length into memory just by claiming to be an
    /// image.
    /// </summary>
    ///
    /// <param name="file">The upload to read from.</param>
    /// <param name="cancellationToken">The token cancelling the read.</param>
    ///
    /// <returns>
    /// The bytes actually read, trimmed to that length when the stream ended early, so a truncated file yields a short buffer rather than
    /// one padded with zeroes that a parser would misread as data.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task<byte[]> ReadHeaderAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var length = (int)Math.Min(file.Length, MaximumHeaderBytes);
        var buffer = new byte[length];

        await using Stream stream = file.OpenReadStream();

        var offset = 0;

        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);

            if (read == 0) break;

            offset += read;
        }

        return offset == buffer.Length ? buffer : buffer[..offset];
    }

    /// <summary>
    /// Tries each supported format in turn, stopping at the first whose signature matches, so a file is parsed once by the one reader that
    /// recognizes it.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns>The dimensions from the first matching format, or <c>null</c> when none matched.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static ImageDimensions? TryRead(ReadOnlySpan<byte> header)
        => TryReadPng(header, out ImageDimensions dimensions)
           || TryReadGif(header, out dimensions)
           || TryReadJpeg(header, out dimensions)
           || TryReadWebP(header, out dimensions)
            ? dimensions
            : null;

    /// <summary>
    /// Reads PNG dimensions from the IHDR chunk, whose width and height sit at fixed offsets in big-endian order.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Reads GIF dimensions from the logical screen descriptor, which stores them little-endian immediately after the signature.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Reads JPEG dimensions by walking the marker segments to the start-of-frame, since JPEG carries no fixed-offset size and the frame
    /// may sit behind any number of metadata segments.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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

        var offset = 2;

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
    /// Reads WebP dimensions by locating the chunk that carries them, which differs between the lossy, lossless, and extended forms.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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

        var offset = 12;

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
    /// Reads the extended WebP form, whose chunk stores each dimension minus one across three bytes.
    /// </summary>
    ///
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Reads the lossless WebP form, whose dimensions are packed as fourteen bits each inside a single little-endian word.
    /// </summary>
    ///
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Reads the lossy WebP form, whose dimensions follow the start code as fourteen-bit values with two scaling bits above them.
    /// </summary>
    ///
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Accepts a parsed pair only when both are positive, so a header that decoded to zero or a negative number is treated as unreadable
    /// rather than as an image of no size.
    /// </summary>
    ///
    /// <param name="width">The width read from the header.</param>
    /// <param name="height">The height read from the header.</param>
    /// <param name="dimensions">Receives the pair when both are positive; otherwise a zero pair the caller must not read.</param>
    ///
    /// <returns><c>true</c> when both were positive; otherwise <c>false</c>.</returns>
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
    /// Checks for the PNG signature and the IHDR chunk together, because the signature alone does not guarantee the size chunk is the one
    /// at the offset this reader uses.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    ///
    /// <returns><c>true</c> when the header is PNG; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool HasPngHeader(ReadOnlySpan<byte> header)
    {
        if (header.Length < 24)
            return false;

        return header[..8].SequenceEqual(PngSignature) && header.Slice(12, 4).SequenceEqual("IHDR"u8);
    }

    /// <summary>
    /// Checks for either GIF version marker, both of which carry the screen descriptor at the same offset.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
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
    /// Checks for the start-of-image marker every JPEG opens with, before any segment walking begins.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
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
    /// Checks whether a marker is one of the standalone ones, which the walk must step over by two bytes rather than by a segment length.
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
        if (StandaloneJpegMarkers.Contains(marker))
            return true;

        return marker is >= 0xD0 and <= 0xD7;
    }

    /// <summary>
    /// Guards the segment walk against a length word that would step past the bytes read, which a truncated or hostile file can produce.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
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
    /// Checks for the RIFF container and its WebP form marker, which sit either side of the four-byte file length.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
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
    /// Guards the chunk walk against a size that would step past the bytes read, on the same terms as the JPEG segment check.
    /// </summary>
    ///
    /// <param name="header">The leading bytes read from the file, which may be shorter than asked for when the file was truncated.</param>
    /// <param name="dataOffset">Where the chunk payload starts, past the marker and size that precede it.</param>
    /// <param name="chunkSize">
    /// The size the chunk header declared, which is checked against the bytes actually read before it is trusted.
    /// </param>
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
    /// Dispatches a chunk to the reader for its form, since the three WebP encodings store dimensions in three different layouts.
    /// </summary>
    ///
    /// <param name="chunkType">The four-character chunk marker, which decides the layout its dimensions are stored in.</param>
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
    /// <param name="dimensions">Receives the pair when one could be read; otherwise a zero pair the caller must not read.</param>
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
    /// Checks for the lossless chunk marker and the signature byte that follows its header.
    /// </summary>
    ///
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
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
    /// Checks for the lossy chunk marker and the three-byte start code that precedes its dimensions.
    /// </summary>
    ///
    /// <param name="chunk">The chunk to read, positioned at its header so the dimensions sit at the offsets this form uses.</param>
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
    /// Reads a three-byte little-endian value, a width the framework offers no primitive for and which the extended WebP form uses.
    /// </summary>
    ///
    /// <param name="value">The bytes to read, positioned at the field this format stores its number in.</param>
    ///
    /// <returns>The integer value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static int ReadUInt24LittleEndian(ReadOnlySpan<byte> value) => value[0] | (value[1] << 8) | (value[2] << 16);

    /// <summary>
    /// Checks whether a marker begins the frame whose segment holds the dimensions, which is the point the walk is looking for.
    /// </summary>
    ///
    /// <param name="marker">The JPEG marker.</param>
    ///
    /// <returns><c>true</c> when the marker contains dimensions; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsStartOfFrameMarker(byte marker) => StartOfFrameJpegMarkers.Contains(marker);
}
