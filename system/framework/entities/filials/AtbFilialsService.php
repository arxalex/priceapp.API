<?php

namespace framework\entities\filials;

use framework\entities\default_entities\DefaultEntitiesService;
use framework\database\Request;
use framework\utils\RadiansHelper;
use PDO;

class AtbFilialsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "filials\\AtbFilial";
        $this->tableName = "pa_filials_atb";
    }

    public function getFilialsByCord(float $xCord, float $yCord, int $radius): array
    {
        $earthRadius = 6372.795;
        $deltaCord = (float) $radius / ($earthRadius * cos($yCord * 2 * pi() / 360) * 2.0 * pi()) * 360.0;
        $minXCord = $xCord - $deltaCord > -180 ? $xCord - $deltaCord : $xCord - $deltaCord + 360;
        $maxXCord = $xCord + $deltaCord <= 180 ? $xCord + $deltaCord : $xCord + $deltaCord - 360;
        $minYCord = $yCord - $deltaCord > -90 ? $yCord - $deltaCord : -90;
        $maxYCord = $yCord + $deltaCord <= 90 ? $yCord + $deltaCord : 90;

        $table = $this->tableName;
        $whereQuery = "`xcord` > $minXCord AND `xcord` < $maxXCord AND `ycord` > $minYCord AND `ycord` < $maxYCord";
        $query = "select * from `$table` where $whereQuery";

        $connection = new Request($query);
        $connection->execute();
        $response = $connection->fetchAll(PDO::FETCH_CLASS, $this->className);

        $result = [];

        foreach ($response as $value) {
            if (RadiansHelper::getLength($xCord, $yCord, $value->xcord, $value->ycord) <= $radius) {
                $result[] = $value;
            }
        }

        return $result;
    }
}
