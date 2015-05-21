namespace Telerik.Core
{
	internal static class CommonStrings
	{
		public const string AssemblyProduct = "Telerik UI for Windows Universal";
		public const string AssemblyCommonDescription = " for Windows Universal applications";
#if TRIAL
        public const string TrialVersionTitleString = " (Trial Version)";
#else
		public const string TrialVersionTitleString = "";
#endif

#if WINDOWS_APP
		public const string FrameworkTitleString = "Windows Universal Applications";
#elif WINDOWS_PHONE_APP
		public const string FrameworkTitleString = "Windows Phone 8.1 Applications";
#else
		public const string FrameworkTitleString = "";
#endif
	}
}
