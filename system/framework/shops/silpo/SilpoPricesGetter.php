<?php

namespace framework\shops\silpo;

use framework\database\NumericHelper;

class SilpoPricesGetter
{   
    public function getItemPriceAndQuantity(int $inshopid, int $silpoFilialId = 2043) : PriceAndQuantitySilpo
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'silpoFilialId' => $silpoFilialId,
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
            $priceAndQuantity = new PriceAndQuantitySilpo(NumericHelper::toFloat($result->item->price), NumericHelper::toFloat($result->item->quantity), $silpoFilialId);
        } else {
            $priceAndQuantity = new PriceAndQuantitySilpo(0, 0, $silpoFilialId);
        }

        return $priceAndQuantity;
    }
}
class PriceAndQuantitySilpo{
    public float $price;
    public float $quantity;
    public int $filialid;

    public function __construct(
        float $price,
        float $quantity,
        int $filialid
    )
    {
        $this->price = $price;
        $this->quantity = $quantity;
        $this->filialid = $filialid;
    }
}
