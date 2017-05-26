﻿using ClassLibrary1;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using ServerForGame.Classes;
using ServerForGame.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace ServerForGame.Controllers
{
    public class HomeController : Controller
    {

        public static WorkerBox workerbox { get; set; } = new WorkerBox();
        public static string AppPath { get; set; } = HostingEnvironment.ApplicationPhysicalPath + @"\StatisticFile\stats.json";

        public ActionResult Index()
        {
            //Попытаться загрузить данные с файла, если он пустой, то заполнить его
            return View(workerbox.statisticWorker.ValidateData(AppPath));
        }

        //ServerObject.Kod:
        //0: ServerObject.Login1-победа, ServerObject.Login2-поражение
        //1: ServerObject.Login1-поражение,ServerObject.Login2-победа
        //2: Обоим логинам засчитать ничью
        [HttpPost]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteData(ServerObject obj)
        {
            workerbox.statisticWorker.SetWinOrWon(obj, AppPath);
            
        }

        [HttpPost]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetData()
        {
            //отправка статистики в JSON по запросу
            return workerbox.statisticWorker.PostData(AppPath);
           
        }

       
    }
}