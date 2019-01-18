using System; 
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Logging; 
using Quartz.Impl;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Walt.Framework.Quartz
{

    public class JobUpdateListens : IJobListener
    { 
        public string Name => "jobupdateListens";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
             QuartzDbContext db =Program.Host.Services.GetService<QuartzDbContext>();
                            var item = db.QuartzTask.FirstOrDefault(w => w.IsDelete == 0
                            &&w.TaskName==context.JobDetail.Key.Name
                            &&w.GroupName==context.JobDetail.Key.Group);
            item.Status=(int) TaskStatus.WaitingToRun;
            db.Update<QuartzTask>(item);
            db.SaveChanges();
             return Task.FromResult(true);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            TaskStatus status= TaskStatus.Running;
             QuartzDbContext db =Program.Host.Services.GetService<QuartzDbContext>();
            var logFaoctory =Program.Host.Services.GetService<ILoggerFactory>();
            var log=logFaoctory.CreateLogger<JobUpdateListens>();
             var item = db.QuartzTask.FirstOrDefault(w => w.IsDelete == 0
                                                     &&w.TaskName==context.JobDetail.Key.Name
                                                     &&w.GroupName==context.JobDetail.Key.Group);
            if(jobException!=null)
            {
                item.Status=(int)TaskStatus.Faulted;
                item.Remark=Newtonsoft.Json.JsonConvert.SerializeObject(jobException);
                log.LogError("Job执行错误,name：{0},Group:{1}",context.JobDetail.Key.Name,context.JobDetail.Key.Group);
            }
            else
            {
                item.Status=(int)TaskStatus.RanToCompletion;
                item.RecentRunTime= context.FireTimeUtc.DateTime;
                if(context.NextFireTimeUtc.HasValue)
                {
                     item.NextFireTime=context.NextFireTimeUtc.Value.DateTime;
                }
            }
            db.Update<QuartzTask>(item);
            db.SaveChanges();
             return Task.FromResult(true); 
        }
    }

}