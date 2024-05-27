using Microsoft.Extensions.Logging;
using PgpCore;
using System.Collections;
using System.IO;
using System.Text;


namespace FileStoragePOC;

/// <summary>
/// This class is used for encrypting flat file after file generation. 
/// </summary>
public class EncryptFileServices 
{
   // private readonly ILogger _logger;

    const bool ARMOR = true;
    const bool WITH_INTEGRITY_CHECK = true;


    /// <summary>
    /// Constructor to initialize objects.
    /// </summary>
    /// <param name="logger"></param>
    public EncryptFileServices()
    {
    }


    /// <summary>
    /// To get the list of public keys from key folder.
    /// this will return list of public keys as stream. 
    /// </summary>
    public async Task<byte[]> DecryptFile(byte[] inputBytes)
    {
        Stream inputStream = new MemoryStream(inputBytes);

        Stream outputStream = new MemoryStream();
        const string PASS_PHRASE = "P@ssw0rd";

        var PRIVATE_KEY_STREAM = GetPrivateDecryptionKeys();

        using (var pgp = new PGP())
        {

            await pgp.DecryptStreamAsync(inputStream, outputStream, PRIVATE_KEY_STREAM, PASS_PHRASE);

        }

        outputStream.Position = 0;
        byte[] outputBytes;
        
        using (var reader = new StreamReader(outputStream))
        {
            outputBytes = System.Text.Encoding.GetEncoding(1252).GetBytes(reader.ReadToEnd());
        }
        return outputBytes;
    }

    /// <summary>
    /// This method converting private key into stream. And this is returning stream of private key for the testing 
    /// </summary>

    public Stream GetPrivateDecryptionKeys()
    {
        #region Soumyajit_private_key
        string key = @"-----BEGIN PGP PRIVATE KEY BLOCK-----

        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
        NGB+Bx9fV8iXvB7L877d
        =5h+n
        -----END PGP PRIVATE KEY BLOCK-----
        ";
        #endregion

        MemoryStream privateKeyStream = new MemoryStream(Encoding.GetEncoding(1252).GetBytes(key));
        return privateKeyStream;
    }

    /// <summary>
    /// This method is returning public key for the testing 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Stream> GetPublicTestKey()
    {

        #region TestPublicKeys
        string key = @"-----BEGIN PGP PUBLIC KEY BLOCK-----

        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
        =TGE+
        -----END PGP PUBLIC KEY BLOCK-----
        ";

        #endregion 

        MemoryStream privateKeyStream = new MemoryStream(Encoding.GetEncoding(1252).GetBytes(key));
        var streams = new List<Stream>();

        streams.Add(privateKeyStream);

        return streams;


    }

    /// <summary>
    /// This method will be used for encrypt the flat file after file generation
    /// </summary>
    /// <param name="inputBytes">input file content in byte array </param>
    /// <param name="encryptionKey">collection of public key stream </param>
    /// <returns>encrypted file content in byte array</returns>
    public async Task<byte[]> EncryptFile(byte[] inputBytes, IEnumerable<Stream> encryptionKey)
    {
        Stream inputStream = new MemoryStream(inputBytes);

        using (var pgp = new PGP())
        {
            Stream outputStream = new MemoryStream();


            await pgp.EncryptStreamAsync(inputStream,
                                         outputStream,
                                         encryptionKey,
                                         ARMOR,
                                         WITH_INTEGRITY_CHECK);

            outputStream.Position = 0;
            byte[] outputBytes;

            using (var reader = new StreamReader(outputStream))
            {
                outputBytes = System.Text.Encoding.GetEncoding(1252).GetBytes(reader.ReadToEnd());
            }
            return outputBytes;

        }

    }

}
