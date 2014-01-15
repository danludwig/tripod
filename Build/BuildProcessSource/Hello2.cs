//using System.Activities;
//using Microsoft.TeamFoundation.Build.Client;
//using Microsoft.TeamFoundation.Build.Workflow.Activities;
//using Microsoft.TeamFoundation.Build.Workflow.Tracking;

//namespace BuildProcessSource
//{
//    // enable the build process template to load the activity
//    [BuildActivity(HostEnvironmentOption.All)]
//    // keep the internal activity operations from appearing in the log
//    [ActivityTracking(ActivityTrackingOption.ActivityOnly)]
//    public sealed class Hello2 : Activity
//    {
//        // Define an activity input argument of type string
//        public InArgument<string> SayHelloTo { get; set; }

//        // If your activity returns a value, derive from CodeActivity<TResult>
//        // and return the value from the Execute method.
//        protected override void Execute(CodeActivityContext context)
//        {
//            // Obtain the runtime value of the Text input argument
//            string text = context.GetValue(this.SayHelloTo);

//            // Add our default value if we did not get one
//            if (string.IsNullOrEmpty(text))
//            {
//                text = "World";
//            }

//            // Write the message to the log
//            context.TrackBuildWarning("Hello " + text, BuildMessageImportance.High);
//        }
//    }
//}