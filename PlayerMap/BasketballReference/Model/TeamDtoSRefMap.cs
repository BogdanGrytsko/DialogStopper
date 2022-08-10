using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMap.BasketballReference.Model
{
    internal class TeamDtoSRefMap : ClassMap<TeamDto>
    {
        public TeamDtoSRefMap()
        {
            Map(x => x.BBRefTeamId).Name("BBRefTeamId");
            Map(x => x.TeamName).Name("TeamName");
            Map(x => x.MongoTeamId).Name("MongoTeamId");
        }
    }
}

