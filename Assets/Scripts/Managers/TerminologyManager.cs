public class TerminologyManager
{
    public const string STATE_POINT = "以太值";
    public const string STATE_BREAK = "以太崩潰";
    public const string STRESS_VALUE = "負荷值";
    public const string STRESS_BREAK = "負荷崩潰";
    public const string KNOCKOUT = "擊飛";

    public static string GetSpeedLevelText( int speedLevel )
    {
        string _speedLevelText = "";

        switch ( speedLevel )
        {
            case 1:

                _speedLevelText = "普速";

                break;

            case 2:

                _speedLevelText = "快速";

                break;

            case 3:

                _speedLevelText = "迅速";

                break;

            case 4:

                _speedLevelText = "神速";

                break;
        }

        return _speedLevelText;
    }
}
