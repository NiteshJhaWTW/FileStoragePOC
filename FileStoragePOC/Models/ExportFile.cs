﻿using FileStoragePOC.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStoragePOC.Models
{
    public class ExportFile
    {
        public ExportFileType FileType { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
