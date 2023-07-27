﻿using Aquc.Stackbricks.MsgPvder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquc.Stackbricks;

public interface IStackbricksMsgPvder
{
    public string MsgPvderId { get; }
    public Task<StackbricksUpdateMessage> GetUpdateMessageAsync(StackbricksManifest stackbricksManifest);
}
public class StackbricksUpdateMessage
{
    public StackbricksManifest stackbricksManifest;
    public Version version;
    public string PkgPvderId;
    public string PkgPvderArgs;
    public StackbricksUpdateMessage(StackbricksManifest stackbricksManifest, Version version, string pkgPvderId, string pkgPvderArgs)
    {
        this.stackbricksManifest = stackbricksManifest;
        this.version = version;
        PkgPvderId = pkgPvderId;
        PkgPvderArgs = pkgPvderArgs;
    }
    public IStackbricksPkgPvder GetPkgPvder() => StackbricksPkgPvderManager.ParsePkgPvder(PkgPvderId);
    public bool NeedUpdate() => version > stackbricksManifest.Version;
}
public class StackbricksMsgPvderManager
{
    static readonly Dictionary<string, IStackbricksMsgPvder> matchDict = new()
    {
        {BiliCommitMsgPvder.ID,new BiliCommitMsgPvder() },
        {WeiboCommitMsgPvder.ID, new WeiboCommitMsgPvder() }
    };
    public static IStackbricksMsgPvder ParseMsgPvder(string msgPvderId)
    {
        // ncpe
        return matchDict[msgPvderId];
    }
}
