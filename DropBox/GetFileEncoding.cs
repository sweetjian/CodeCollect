/// <summary>
        ///Return the Encoding of a text file.  Return Encoding.Default if no Unicode
        // BOM (byte order mark) is found.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static Encoding GetFileEncoding(String FileName)
        {
            Encoding Result = null;
            FileInfo FI = new FileInfo(FileName);
            FileStream FS = null;
            try
            {
                FS = FI.OpenRead();
                Encoding[] UnicodeEncodings =
                { 
                    Encoding.BigEndianUnicode, 
                    Encoding.Unicode,
                    Encoding.UTF8,
                    Encoding.UTF32,
                    new UTF32Encoding(true,true)
                };
                for (int i = 0; Result == null && i < UnicodeEncodings.Length; i++)
                {
                    FS.Position = 0;
                    byte[] Preamble = UnicodeEncodings[i].GetPreamble();
                    bool PreamblesAreEqual = true;
                    for (int j = 0; PreamblesAreEqual && j < Preamble.Length; j++)
                    {
                        PreamblesAreEqual = Preamble[j] == FS.ReadByte();
                    }
                    // or use Array.Equals to compare two arrays.
                    // fs.Read(buf, 0, Preamble.Length);
                    // PreamblesAreEqual = Array.Equals(Preamble, buf)
                    if (PreamblesAreEqual)
                    {
                        Result = UnicodeEncodings[i];
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                throw ex;
            }
            finally
            {
                if (FS != null)
                {
                    FS.Close();
                }
            }
            if (Result == null)
            {
                Result = Encoding.Default;
            }
            return Result;
        }
