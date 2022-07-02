<?php

namespace framework\shops\silpo;

use framework\database\NumericHelper;

class SilpoPricesGetter
{
    public function getItemPriceAndQuantity(int $inshopid, int $silpoFilialId = 2043): PriceAndQuantitySilpo
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'filialId' => $silpoFilialId,
                "skuid" => $inshopid
            ],
            'method' => 'GetDetailedCatalogItem'
        ]);

        $options = [
            'http' => [
                'header'  => "Content-Type: application/json;charset=UTF-8\r\n",
                'method'  => 'POST',
                'content' => $data
            ]
        ];
        $context  = stream_context_create($options);
        $result = json_decode(file_get_contents($url, false, $context));

        if ($result->item != null) {
            $priceAndQuantity = new PriceAndQuantitySilpo(NumericHelper::toFloat($result->item->price), NumericHelper::toFloat($result->item->quantity), $silpoFilialId);
        } else {
            $priceAndQuantity = new PriceAndQuantitySilpo(0, 0, $silpoFilialId);
        }

        return $priceAndQuantity;
    }
    public function getPricesAndQuantitiesByCategory(array $inshopcategories, int $silpoFilialId = 2043): array
    {
        $items = [];
        foreach ($inshopcategories as $inshopcategory) {
            $inshopcategoryid = $inshopcategory->categoryshopid;
            $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
            $data = json_encode([
                'data' => [
                    'filialId' => $silpoFilialId,
                    'From' => 0,
                    'To' => 10000,
                    'categoryId' => $inshopcategoryid
                ],
                'method' => 'GetSimpleCatalogItems'
            ]);

            $options = [
                'http' => [
                    'header'  => "Content-Type: application/json;charset=UTF-8\r\n",
                    'method'  => 'POST',
                    'content' => $data
                ]
            ];
            $context  = stream_context_create($options);
            $result = json_decode(file_get_contents($url, false, $context));

            $items = array_merge($result->items, $items);
        }

        $result = [];
        foreach ($items as $item) {
            $priceAndQuantity = new PriceAndQuantitySilpo(NumericHelper::toInt($item->id), NumericHelper::toFloat($item->price), NumericHelper::toFloat($item->quantity));
            $result[] = $priceAndQuantity;
        }

        return $result;
    }
}
class PriceAndQuantitySilpo
{
    public int $inshopid;
    public float $price;
    public float $quantity;

    public function __construct(
        int $inshopid,
        float $price,
        float $quantity
    ) {
        $this->inshopid = $inshopid;
        $this->price = $price;
        $this->quantity = $quantity;
    }
}
