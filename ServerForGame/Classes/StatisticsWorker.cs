﻿using ClassLibrary1;
using Newtonsoft.Json;
using ServerForGame.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerForGame.Classes
{
    public class StatisticsWorker:IStatisticWorker
    {
        public IJSONWorker<Statistic> JsonWorker { get; set; }
        public IHubWorker HubWorker { get; set; }
        public StatisticsWorker(IJSONWorker<Statistic> JsonWorker,IHubWorker HubWorker)
        {
            this.JsonWorker = JsonWorker;
            this.HubWorker = HubWorker;
        }

        public List<Statistic> ValidateData(string path)
        {
            List<Statistic> statistics;
            statistics = JsonWorker.GetData(path);
            if (statistics == null) statistics = new List<Statistic>();
            if (statistics.Count < 2)
            {
                if (statistics.Where(a => a.Login == "Пользователь").Count() == 0) statistics.Add(new Statistic() { Login = "Пользователь", Win = 0, Won = 0, NF = 0 });
                if (statistics.Where(a => a.Login == "Компьютер").Count() == 0) statistics.Add(new Statistic() { Login = "Компьютер", Win = 0, Won = 0, NF = 0 });
                JsonWorker.WriteData(statistics,path);
            }
            return statistics;
        }

        public void SetWinOrWon(ServerObject servObj,string path)
        {
            List<Statistic> statistics = JsonWorker.GetData(path);
            switch (servObj.Kod)
            {
                case 0:
                    SetWin(servObj, statistics);
                    break;
                case 1:
                    SetWon(servObj, statistics);
                    break;
                case 2:
                    SetDraw(servObj, statistics);
                    break;
            }
            JsonWorker.WriteData(statistics, path);
            statistics = JsonWorker.GetData(path);
            HubWorker.BroadcastObject(statistics);//Обновление статистики в браузере
        }
        public string PostData(string path)
        {
            List<Statistic> statistics = JsonWorker.GetData(path);
            return JsonConvert.SerializeObject(statistics);
        }

        #region PrivateSection
        private void SetWin(ServerObject obj,List<Statistic> statistics)
        {
            statistics.FirstOrDefault(a => a.Login == obj.Login1).Win++;
            statistics.FirstOrDefault(a => a.Login == obj.Login2).Won++;
        }

        private void SetWon(ServerObject obj, List<Statistic> statistics)
        {
            statistics.FirstOrDefault(a => a.Login == obj.Login1).Won++;
            statistics.FirstOrDefault(a => a.Login == obj.Login2).Win++;
        }

        private void SetDraw(ServerObject obj, List<Statistic> statistics)
        {
            statistics.FirstOrDefault(a => a.Login == obj.Login1).NF++;
            statistics.FirstOrDefault(a => a.Login == obj.Login2).NF++;
        }
        #endregion

    }
}