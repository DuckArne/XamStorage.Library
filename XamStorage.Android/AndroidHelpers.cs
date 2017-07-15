using AndroidEnvironment = Android.OS.Environment;

namespace XamStorage
{
    public static class AndroidHelpers
{
    public static bool IsExternalStorageWritable()
    {
        string state = AndroidEnvironment.ExternalStorageState;
        if (AndroidEnvironment.MediaMounted.Equals(state))
        {
            return true;
        }
        return false;
    }
}
}