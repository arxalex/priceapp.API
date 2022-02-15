<?php

namespace framework\entities\brands;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class BrandsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "brands\\Brand";
        $this->tableName = "pa_brand";
    }
    public function getBrand(string $label) : Brand
    {
        $labelArr = StringHelper::nameToKeywords($label);
        $brands = $this->getItemsFromDB([
            'label_like' => $labelArr
        ]);
        $rates = StringHelper::rateItemsByKeywords($label, array_column($brands, 'label'));
        return ($this->orderItemsByRate($brands, $rates, 1))[0];
    }
}
