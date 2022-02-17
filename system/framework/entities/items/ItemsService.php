<?php

namespace framework\entities\items;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;

class ItemsService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "items\\Item";
        $this->tableName = "pa_items";
    }
    public function getSimilarItemsByLabel(string $label, ?int $categoryId = null): array
    {
        $labelArr = StringHelper::nameToKeywords($label);
        if ($categoryId !== null) {
            $items = $this->getItemsFromDB([
                'label_like' => $labelArr,
                'categoryid' => [$categoryId]
            ]);
        } else {
            $items = $this->getItemsFromDB([
                'label_like' => $labelArr
            ]);
        }
        echo var_dump($items);
        $rates = StringHelper::rateItemsByKeywords($label, array_column($items, 'label'));
        return $this->orderItemsByRate($items, $rates, 5);
    }
}
