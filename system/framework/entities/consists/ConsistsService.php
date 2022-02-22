<?php

namespace framework\entities\consists;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class ConsistsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "consists\\Consist";
        $this->tableName = "pa_consists";
    }
    public function getConsist(?string $label) : Consist
    {
        if($label == null)
        {
            return $this->getItemFromDB(0);
        }
        $labelArr = StringHelper::nameToKeywords($label);
        $brands = $this->getItemsFromDB([
            'label_like' => $labelArr
        ]);
        $rates = StringHelper::rateItemsByKeywords($label, array_column($brands, 'label'));
        return count($this->orderItemsByRate($brands, $rates, 1)) > 0 ? ($this->orderItemsByRate($brands, $rates, 1))[0] : $this->getItemFromDB(0);
    }
}
