<?php

namespace framework\entities\countries;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class CountriesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "countries\\Country";
        $this->tableName = "pa_countries";
    }
    public function getCountry(?string $label) : Country
    {
        if($label == null)
        {
            return $this->getItemFromDB(0);
        }
        $labelArr = StringHelper::nameToKeywords($label);
        $countries = $this->getItemsFromDB([
            'label_like' => $labelArr
        ]);
        $rates = StringHelper::rateItemsByKeywords($label, array_column($countries, 'label'));
        return count($this->orderItemsByRate($countries, $rates, 1)) > 0 ? ($this->orderItemsByRate($countries, $rates, 1))[0] : $this->getItemFromDB(0);
    }
}
