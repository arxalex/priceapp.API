<?php

namespace framework\entities\items;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;
use framework\database\SqlHelper;
use framework\database\Request;
use PDO;

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
        $rates = StringHelper::rateItemsByKeywords($label, array_column($items, 'label'));
        return $this->orderItemsByRate($items, $rates, 5);
    }
    public function getItemsFromDBByFilials(array $categories, array $filials, ?int $offset = null, ?int $limit = null): array
    {
        $table = $this->tableName;

        $whereQuery = '`category` in ' . SqlHelper::arrayInNumeric($categories) .
            ' and p.filialid in ' . SqlHelper::arrayInNumeric($filials) .
            ' and p.price > 0 ' .
            ' and p.quantity > 0';

        $query = "select i.id, `label`, `image`, `category`, `brand`, `package`, `units`, `term`, `barcodes`, 
            `consist`, `calorie`, `carbohydrates`, `fat`, `proteins`, `additional` 
            from `$table` i 
            left join `pa_prices` p on p.itemid = i.id
            where $whereQuery
            group by i.id";

        if ($offset !== null && $limit !== null) {
            $query .= " LIMIT $limit OFFSET $offset";
        }

        $connection = new Request($query);
        $connection->execute();
        $response = $connection->fetchAll(PDO::FETCH_CLASS, $this->className);
        return $response;
    }
}
