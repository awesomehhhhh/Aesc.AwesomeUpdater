﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquc.Stackbricks;

public class StackbricksUpdatePackage
{
    public StackbricksUpdateMessage updateMessage;
    public DirectoryInfo programDir;
    public string file;
    public DirectoryInfo depressedDir;
    public bool isZip;

    public const string FILE_PKGCFG = "Aquc.Stackbricks.pkgcfg.json";

    public StackbricksUpdatePackage(string file, StackbricksUpdateMessage updateMessage, bool isZip = true)
    {
        this.isZip = isZip;
        this.file = file;
        this.updateMessage = updateMessage;
        programDir = updateMessage.stackbricksManifest.ProgramDir;
        depressedDir = isZip ? DepressedZipFile() : programDir;
    }
    protected DirectoryInfo DepressedZipFile()
    {
        var depressedDir = new DirectoryInfo(Path.Combine(programDir.FullName, $".StackbricksUpdatePackage_{updateMessage.version}.depressed"));
        if (!depressedDir.Exists) depressedDir.Create();
        else
        {
            depressedDir.Delete(true);
            depressedDir.Create();
        }
        ZipFile.ExtractToDirectory(file, depressedDir.FullName);
        StackbricksProgram.logger.Debug($"Extract file successfully, to={depressedDir.FullName}");
        return depressedDir;
    }
    public void ExecuteActions()
    {
        var pkgcfg = isZip ? depressedDir.GetFiles(FILE_PKGCFG) : Array.Empty<FileInfo>();
        StackbricksActionList stackbricksActionList;
        if (pkgcfg.Length == 0)
        {
            stackbricksActionList = new StackbricksActionList(updateMessage.stackbricksManifest.UpdateActions);
        }
        else
        {
            stackbricksActionList = new StackbricksActionList(pkgcfg[0].FullName);
        }
        stackbricksActionList.ExecuteList(this);
    }
}
