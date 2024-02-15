using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Nexu.Shared.Infrastructure.Security
{
    public static class RsaCommon
    {
        private const char Marker = '-';
        private const string BeginMarker = "--BEGIN ";
        private const string EndMarker = "--END ";
        private const string RsaPrivateKeyHeader = "RSA PRIVATE KEY";
        private const string EncryptedPrivateKeyHeader = "ENCRYPTED PRIVATE KEY";
        private const string PrivateKeyHeader = "PRIVATE KEY";
        private const string PublicKeyHeader = "PUBLIC KEY";
        private const string RsaPublicKeyHeader = "RSA PUBLIC KEY";

        public static RSA FromPemPrivateKey(string content, string password)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return FromPrivatePemInternal(content, password);
        }

        public static RSA FromPemPublicKey(string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return FromPublicPemInternal(content);
        }

        private static RSA FromPublicPemInternal(string content)
        {
            var span = content.AsSpan();
            var start = span.IndexOf(BeginMarker);
            string body;
            Action<RSA, byte[]> loader;
            if (start >= 0)
            {
                span = span.Slice(start + BeginMarker.Length);

                string type;
                if (span.IndexOf(RsaPublicKeyHeader) == 0)
                {
                    type = RsaPublicKeyHeader;
                    loader = (rsa, body) => rsa.ImportRSAPublicKey(body, out _);
                }
                else if (span.IndexOf(PublicKeyHeader) == 0)
                {
                    type = PublicKeyHeader;
                    loader = (rsa, body) => rsa.ImportSubjectPublicKeyInfo(body, out _);
                }
                else
                {
                    throw new ArgumentException("Unknown private key format.");
                }

                span = span.Slice(type.Length);
                // Move past the header
                var next = span.IndexOf(Marker);
                while (span.Length > next && span[next] == Marker)
                {
                    next++;
                }

                span = span.Slice(next);
                body = GetBody(span, EndMarker + type);
            }
            else
            {
                // Legacy: The contents of the key were passed directly
                body = content;
                loader = (rsa, body) => rsa.ImportSubjectPublicKeyInfo(body, out _);
            }

            var bytes = Convert.FromBase64String(body);
            var rsa = RSA.Create();
            try
            {
                loader(rsa, bytes);
                return rsa;
            }
            catch
            {
                rsa.Dispose();
                throw;
            }
        }

        private static RSA FromPrivatePemInternal(string content, string password)
        {
            var span = content.AsSpan();
            var start = span.IndexOf(BeginMarker);
            Action<RSA, byte[]> loader;
            string body;
            if (start >= 0)
            {
                span = span.Slice(start + BeginMarker.Length);

                string type;
                if (span.IndexOf(RsaPrivateKeyHeader) == 0)
                {
                    if (span.IndexOf("Proc-Type: 4,ENCRYPTED") > 0)
                    {
                        // We can implement the decryption later if needed
                        throw new ArgumentException("Encrypted PEM files not supported.");
                    }

                    type = RsaPrivateKeyHeader;
                    loader = (rsa, body) => rsa.ImportRSAPrivateKey(body, out _);
                }
                else if (span.IndexOf(EncryptedPrivateKeyHeader) == 0)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        throw new ArgumentException("Password is required for encrypted RSA key.");
                    }

                    type = EncryptedPrivateKeyHeader;
                    loader = (rsa, body) => rsa.ImportEncryptedPkcs8PrivateKey(password, body, out _);
                }
                else if (span.IndexOf(PrivateKeyHeader) == 0)
                {
                    type = PrivateKeyHeader;
                    loader = (rsa, body) => rsa.ImportPkcs8PrivateKey(body, out _);
                }
                else
                {
                    throw new ArgumentException("Unknown private key format.");
                }

                span = span.Slice(type.Length);
                // Move past the header
                var next = span.IndexOf(Marker);
                while (span.Length > next && span[next] == Marker)
                {
                    next++;
                }

                span = span.Slice(next);
                body = GetBody(span, EndMarker + type);
            }
            else
            {
                // Legacy: The contents of the key were passed directly
                body = content;
                loader = (rsa, body) => rsa.ImportRSAPrivateKey(body, out _);
            }

            var bytes = Convert.FromBase64String(body);
            var rsa = RSA.Create();
            try
            {
                loader(rsa, bytes);
                return rsa;
            }
            catch
            {
                rsa.Dispose();
                throw;
            }
        }

        private static string GetBody(ReadOnlySpan<char> span, string endMarker)
        {
            var builder = new StringBuilder(span.Length);
            for (var i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if (c == Marker)
                {
                    var last = span.Slice(i);
                    var endIndex = last.IndexOf(endMarker);
                    Debug.Assert(endIndex >= 0);
                    // We reached the end of the content
                    break;
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
