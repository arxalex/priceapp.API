<?php

namespace framework\utils;

class RadiansHelper
{
    public static function getLength(float $xCord1, float $yCord1, float $xCord2, float $yCord2) : float
    {
        $rad = 6372795;

        $lat1 = $yCord1 * pi() / 180.0;
        $lat2 = $yCord2 * pi() / 180.0;
        $long1 = $xCord1 * pi() / 180.0;
        $long2 = $xCord2 * pi() / 180.0;

        $cl1 = cos($lat1);
        $cl2 = cos($lat2);
        $sl1 = sin($lat1);
        $sl2 = sin($lat2);
        $delta = $long2 - $long1;
        $cdelta = cos($delta);
        $sdelta = sin($delta);

        $y = sqrt(pow($cl2 * $sdelta, 2) + pow($cl1 * $sl2 - $sl1 * $cl2 * $cdelta, 2));
        $x = $sl1 * $sl2 + $cl1 * $cl2 * $cdelta;
        $ad = atan2($y, $x);
        $dist = $ad * $rad;

        return $dist;
    }
}
