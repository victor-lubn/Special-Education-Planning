
//using microsoft.aspnetcore.mvc.filters;
//using system;
//using system.collections.generic;
//using system.linq;
//using system.threading.tasks;

//namespace SpecialEducationPlanning
.api.interceptors
//{
//    /// <summary>
//    /// the action logger looking after messages getting in and out of the controllers
//    /// </summary>
//    public class actionlogger : actionfilterattribute
//    {
//        /// <summary>
//        /// the serilog logger
//        /// </summary>
//        private readonly serilog.ilogger logger;


//        /// <summary>
//        /// create a new instance of <see cref="actionlogger"/>
//        /// </summary>
//        /// <param name="logger"></param>
//        public actionlogger(serilog.ilogger logger)
//        {
//            this.logger = logger;
//        }

//        /// <summary>
//        /// logs when an action is about to be executed
//        /// </summary>
//        /// <param name="context">action context</param>
//        public override void onactionexecuting(actionexecutingcontext context)
//        {
//            const string message = "executing action: {action} arguments: {arguments}";
//            var arguments = context.actionarguments?.select(d => new { name = d.key, d.value });
//            logger.debug(message, context.actiondescriptor.displayname, arguments);
//            base.onactionexecuting(context);
//        }

//        /// <summary>
//        /// logs when an action finished
//        /// </summary>
//        /// <param name="context">action context</param>
//        public override void onactionexecuted(actionexecutedcontext context)
//        {
//            if (context.exception != null)
//            {
//                const string message = "error executing action: {action}";
//                logger.error(context.exception, message, context.controller, context.actiondescriptor.displayname);
//            }
//            else
//            {
//                const string message = "action completed: {action}";
//                logger.debug(message, context.controller, context.actiondescriptor.displayname);
//            }

//            base.onactionexecuted(context);
//        }

//        ///// <summary>
//        ///// logs when a result evalution is about to start
//        ///// </summary>
//        ///// <param name="context">result evaluation context</param>
//        //public override void onresultexecuting(resultexecutingcontext context)
//        //{
//        //    const string message = "result executing action: {action}";
//        //    logger.debug(message, context.actiondescriptor.displayname);
//        //    base.onresultexecuting(context);
//        //}

//        ///// <summary>
//        ///// logs when a result evaluation has finised
//        ///// </summary>
//        ///// <param name="context">result evaluation context</param>
//        //public override void onresultexecuted(resultexecutedcontext context)
//        //{
//        //    if (context.exception != null)
//        //    {
//        //        const string message = "error on result action: {action}";
//        //        logger.error(context.exception, message, context.actiondescriptor.displayname);
//        //    }
//        //    else
//        //    {
//        //        const string message = "result completed succesfuly action: {action}";
//        //        logger.debug(message, context.actiondescriptor.displayname);
//        //    }

//        //    base.onresultexecuted(context);
//        //}
//    }
//}
