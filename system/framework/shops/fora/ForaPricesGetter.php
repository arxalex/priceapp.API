<?php

namespace framework\shops\fora;

use framework\database\NumericHelper;

class ForaPricesGetter
{   
    public function getItemPriceAndQuantity(int $inshopid, int $foraFilialId = 310) : PriceAndQuantityFora
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'filialId' => $foraFilialId,
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

        if($result->item != null){
            $priceAndQuantity = new PriceAndQuantityFora(NumericHelper::toFloat($result->item->price), NumericHelper::toFloat($result->item->quantity), $foraFilialId);
        } else {
            $priceAndQuantity = new PriceAndQuantityFora(0, 0, $foraFilialId);
        }

        return $priceAndQuantity;
    }
    public function getPricesAndQuantitiesByCategory(int $inshopcategoryid, int $foraFilialId = 310) : array
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'filialId' => $foraFilialId,
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

        $items = $result->items;

        $result = [];
        foreach($items as $item){
            $priceAndQuantity = new PriceAndQuantityFora(NumericHelper::toInt($item->id), NumericHelper::toFloat($item->price), NumericHelper::toFloat($item->quantity));
            $result[] = $priceAndQuantity;
        }

        return $result;
    }
}
class PriceAndQuantityFora{
    public int $inshopid;
    public float $price;
    public float $quantity;

    public function __construct(
        int $inshopid,
        float $price,
        float $quantity
    )
    {
        $this->inshopid = $inshopid;
        $this->price = $price;
        $this->quantity = $quantity;
    }
}
