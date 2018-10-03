using NowMine.Models;
using NowMineCommon.Models;
using System.Collections.Generic;

namespace NowMine.APIProviders
{
    interface IAPIProvider
    {
        List<ClipInfo> GetSearchClipInfos(string keyWord);
    }
}
