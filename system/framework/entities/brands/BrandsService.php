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
    public function getBrand(?string $label) : Brand
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
