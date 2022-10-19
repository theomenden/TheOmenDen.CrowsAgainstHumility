using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsAgainstHumility.Core.Criteria;
public sealed class SearchUsersCriteria: SearchBase
{
    public String[] Roles { get; set; }
}
