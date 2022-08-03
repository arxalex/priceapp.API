namespace priceapp.API.Utils;

public static class LocationUtil
{
    public static double GetLength(double xCord1, double yCord1, double xCord2, double yCord2)
    {
        const int rad = 6372795;

        var lat1 = yCord1 * Math.PI / 180.0;
        var lat2 = yCord2 * Math.PI / 180.0;
        var long1 = xCord1 * Math.PI / 180.0;
        var long2 = xCord2 * Math.PI / 180.0;

        var cl1 = Math.Cos(lat1);
        var cl2 = Math.Cos(lat2);
        var sl1 = Math.Sin(lat1);
        var sl2 = Math.Sin(lat2);
        var delta = long2 - long1;
        var cDelta = Math.Cos(delta);
        var sDelta = Math.Sin(delta);

        var y = Math.Sqrt(cl2 * sDelta * cl2 * sDelta +
                          (cl1 * sl2 - sl1 * cl2 * cDelta) * (cl1 * sl2 - sl1 * cl2 * cDelta));
        var x = sl1 * sl2 + cl1 * cl2 * cDelta;
        var ad = Math.Atan2(y, x);
        var dist = ad * rad;

        return dist;
    }
}