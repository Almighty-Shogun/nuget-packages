using Microsoft.AspNetCore.Http;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Recognizes an image by the bytes it opens with rather than by what the client claimed it is. A name and a content type are both written
/// by the caller and neither is evidence, so the rule that asks for an image asks here.
/// </summary>
///
/// <remarks>
/// A wider set of formats is recognized than <see cref="ImageDimensionsReader"/> reads, because recognizing a format and locating its
/// dimensions are different jobs. An upload may be accepted as an image here and still report no dimensions there, which is why the
/// dimension rules fail a file they cannot measure while this one does not.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ImageSignature
{
    /// <summary>
    /// The bytes needed to recognize every format here. The longest check reads the ISO container's brand, which sits twelve bytes in;
    /// the rest is headroom so a new signature needs no new read.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int _signatureBytes = 32;

    /// <summary>
    /// The fixed byte signatures, held as fields because a collection expression in an argument position is inferred as a span of
    /// <see cref="int"/> rather than of <see cref="byte"/> .
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] _pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    /// The little-endian TIFF signature, whose byte-order mark and magic number are both read in that order.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] _tiffLittleEndianSignature = [0x49, 0x49, 0x2A, 0x00];

    /// <summary>
    /// The big-endian TIFF signature, which is the same header written the other way round.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] _tiffBigEndianSignature = [0x4D, 0x4D, 0x00, 0x2A];

    /// <summary>
    /// The Windows icon signature: a reserved word of zero followed by the type word that distinguishes an icon from a cursor.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[] _iconSignature = [0x00, 0x00, 0x01, 0x00];

    /// <summary>
    /// The ISO base-media brands that mean the container holds a picture rather than a film, which is what separates an AVIF or HEIC
    /// still from an MP4 sharing the same container.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static readonly byte[][] _stillImageBrands =
    [
        [.. "avif"u8], [.. "avis"u8],
        [.. "heic"u8], [.. "heix"u8], [.. "heim"u8], [.. "heis"u8],
        [.. "hevc"u8],
        [.. "mif1"u8], [.. "msf1"u8]
    ];

    /// <summary>
    /// Reads the front of an upload and reports whether it is a picture.
    /// </summary>
    ///
    /// <param name="file">The upload to inspect. A zero-length file is refused before any stream is opened.</param>
    /// <param name="cancellationToken">The token cancelling the read.</param>
    ///
    /// <returns><c>true</c> when the leading bytes are a recognized image signature; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static async Task<bool> IsImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length <= 0)
            return false;

        var buffer = new byte[(int)Math.Min(file.Length, _signatureBytes)];

        await using Stream stream = file.OpenReadStream();

        var offset = 0;

        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset), cancellationToken);

            if (read == 0) break;

            offset += read;
        }

        return IsImage(buffer.AsSpan(0, offset));
    }

    /// <summary>
    /// Matches the leading bytes against every recognized signature.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file, which may be shorter than asked for.</param>
    ///
    /// <returns><c>true</c> when any signature matched; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsImage(ReadOnlySpan<byte> header)
        => IsPng(header)
           || IsGif(header)
           || IsJpeg(header)
           || IsWebP(header)
           || IsBmp(header)
           || IsTiff(header)
           || IsIcon(header)
           || IsStillImageContainer(header);

    /// <summary>
    /// Checks for the eight-byte PNG signature.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is PNG; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsPng(ReadOnlySpan<byte> header)
        => header.Length >= 8 && header[..8].SequenceEqual(_pngSignature);

    /// <summary>
    /// Checks for either GIF version marker.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is GIF; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsGif(ReadOnlySpan<byte> header)
        => header.Length >= 6 && (header[..6].SequenceEqual("GIF87a"u8) || header[..6].SequenceEqual("GIF89a"u8));

    /// <summary>
    /// Checks for the JPEG start-of-image marker and the marker byte that must follow it.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is JPEG; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsJpeg(ReadOnlySpan<byte> header)
        => header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

    /// <summary>
    /// Checks for the RIFF container and its WebP form marker, which sit either side of the four-byte file length.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is WebP; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsWebP(ReadOnlySpan<byte> header)
        => header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header.Slice(8, 4).SequenceEqual("WEBP"u8);

    /// <summary>
    /// Checks for the two-byte BMP marker, which is the whole of that format's signature.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is BMP; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsBmp(ReadOnlySpan<byte> header) => header.Length >= 2 && header[..2].SequenceEqual("BM"u8);

    /// <summary>
    /// Checks for either TIFF byte order, each of which carries the format's magic number in its own endianness.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is TIFF; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsTiff(ReadOnlySpan<byte> header)
    {
        if (header.Length < 4)
            return false;

        return header[..4].SequenceEqual(_tiffLittleEndianSignature) || header[..4].SequenceEqual(_tiffBigEndianSignature);
    }

    /// <summary>
    /// Checks for the Windows icon header, whose reserved word and type word together identify the format.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns><c>true</c> when the header is an icon; otherwise, <c>false</c>. A cursor shares the layout and is not accepted.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsIcon(ReadOnlySpan<byte> header)
        => header.Length >= 4 && header[..4].SequenceEqual(_iconSignature);

    /// <summary>
    /// Checks for an ISO base-media container whose brand is a still-image one, which is how AVIF and HEIC are recognized.
    /// </summary>
    ///
    /// <param name="header">The bytes read from the front of the file.</param>
    ///
    /// <returns>
    /// <c>true</c> when the container declares a still-image brand; otherwise, <c>false</c>. A film in the same container, such as an
    /// MP4, declares a different brand and is refused.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool IsStillImageContainer(ReadOnlySpan<byte> header)
    {
        if (header.Length < 12 || !header.Slice(4, 4).SequenceEqual("ftyp"u8))
            return false;

        foreach (byte[] brand in _stillImageBrands)
            if (header.Slice(8, 4).SequenceEqual(brand))
                return true;

        return false;
    }
}
