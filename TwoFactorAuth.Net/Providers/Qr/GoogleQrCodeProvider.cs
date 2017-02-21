﻿using System;
using System.Net.Security;

namespace TwoFactorAuthNet.Providers.Qr
{

    /// <summary>
    /// Provides QR codes generated by Google charts.
    /// </summary>
    /// <seealso href="https://developers.google.com/chart/infographics/docs/qr_codes"/>.
    public class GoogleQrCodeProvider : BaseHttpQrCodeProvider, IQrCodeProvider
    {
        /// <summary>
        /// Gets the <see cref="ErrorCorrectionLevel"/> for the QR code.
        /// </summary>
        public ErrorCorrectionLevel ErrorCorrectionLevel { get; private set; }

        /// <summary>
        /// Gets the width of the white border around the data portion of the code.
        /// </summary>
        /// <remarks>
        /// This is in rows, not in pixels.
        /// </remarks>
        public int MarginRows { get; private set; }

        /// <summary>
        /// <see cref="BaseHttpQrCodeProvider.BaseUri"/> for this QR code provider.
        /// </summary>
        private static readonly Uri baseuri = new Uri("https://chart.googleapis.com/chart");

        /// <summary>
        /// Initializes a new instance of a <see cref="GoogleQrCodeProvider"/> with the specified
        /// <see cref="ErrorCorrectionLevel"/>, <see cref="MarginRows"/> and 
        /// <see cref="RemoteCertificateValidationCallback"/>.
        /// </summary>
        /// <param name="errorCorrectionLevel">The <see cref="ErrorCorrectionLevel"/> to use when generating QR codes.</param>
        /// <param name="marginRows">The width of the white border around the data portion of the code.</param>
        /// <param name="remoteCertificateValidationCallback">
        /// The <see cref="RemoteCertificateValidationCallback"/> to use when generating QR codes.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when an invalid <see cref="ErrorCorrectionLevel"/> is specified or marginRows is less than 0.
        /// </exception>
        public GoogleQrCodeProvider(
            ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.Low,
            int marginRows = 1,
            RemoteCertificateValidationCallback remoteCertificateValidationCallback = null
        )
            : base(baseuri, remoteCertificateValidationCallback)
        {
            if (!Enum.IsDefined(typeof(ErrorCorrectionLevel), errorCorrectionLevel))
                throw new ArgumentOutOfRangeException(nameof(errorCorrectionLevel));
            this.ErrorCorrectionLevel = errorCorrectionLevel;

            if (marginRows < 0)
                throw new ArgumentOutOfRangeException(nameof(marginRows));
            this.MarginRows = marginRows;
        }

        /// <summary>
        /// Downloads / retrieves / generates a QR code as image.
        /// </summary>
        /// <param name="text">The text to encode in the QR code.</param>
        /// <param name="size">The desired size (width and height equal) for the image.</param>
        /// <returns>Returns the binary representation of the image.</returns>
        /// <seealso cref="IQrCodeProvider"/>
        public byte[] GetQrCodeImage(string text, int size)
        {
            return this.DownloadData(this.GetUri(text, size));
        }

        /// <summary>
        /// Builds an <see cref="Uri"/> based on the instance's <see cref="BaseHttpQrCodeProvider.BaseUri"/>.
        /// </summary>
        /// <param name="qrText">The text to encode in the QR code.</param>
        /// <param name="size">The desired size of the QR code.</param>
        /// <returns>A <see cref="Uri"/> to the QR code.</returns>
        private Uri GetUri(string qrText, int size)
        {
            return new Uri(this.BaseUri,
                "?cht=qr"
                + "&chs=" + size + "x" + size
                + "&chld=" + (char)this.ErrorCorrectionLevel + "|" + this.MarginRows
                + "&chl=" + Uri.EscapeDataString(qrText)
            );
        }

        /// <summary>
        /// Gets the MIME type of the image.
        /// </summary>
        /// <returns>Returns the MIME type of the image.</returns>
        /// <seealso cref="IQrCodeProvider"/>
        public string GetMimeType()
        {
            return "image/png";
        }
    }
}
