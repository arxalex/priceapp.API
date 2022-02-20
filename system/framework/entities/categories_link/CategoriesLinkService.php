<?php

namespace framework\entities\categories_link;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class CategoriesLinkService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "categories_link\\CategoryLink";
        $this->tableName = "pa_categories_link";
    }
    public function getCategoryLink(?string $label) : CategoryLink
    {
        if($label == null)
        {
            return $this->getItemFromDB(0);
        }
        $labelArr = StringHelper::nameToKeywords($label);
        $categoryLinks = $this->getItemsFromDB([
            'shopcategorylabel_like' => $labelArr
        ]);
        $rates = StringHelper::rateItemsByKeywords($label, array_column($categoryLinks, 'shopcategorylabel'));
        return count($this->orderItemsByRate($categoryLinks, $rates, 1)) > 0 ? ($this->orderItemsByRate($categoryLinks, $rates, 1))[0] : $this->getItemFromDB(0);
    }
}
