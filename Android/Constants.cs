using Android.Views;

namespace Android {
    public static class Constants {
        public const StatusBarVisibility STATUS_BAR_VISIBILITY = (StatusBarVisibility)(SystemUiFlags.LayoutFullscreen | SystemUiFlags.Fullscreen | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);
    }
}