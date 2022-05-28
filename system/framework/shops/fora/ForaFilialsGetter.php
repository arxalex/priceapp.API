<?php

namespace framework\shops\fora;

use framework\database\NumericHelper;
use framework\entities\filials\Filial;
use framework\entities\filials\FilialsService;

class ForaFilialsGetter
{
    private FilialsService $_fillialsService;
    public function __construct()
    {
        $this->_fillialsService = new FilialsService();
    }

    public function getFilials(): array {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'method' => 'GetPickupFilials',
            'data' => [
                "businessId" => 4,
                "merchantId" => 2
            ]
        ]);

        $options = [
            'http' => [
                'header'  => "Content-Type: application/json;charset=UTF-8\r\n",
                'method'  => 'POST',
                'content' => $data
            ]
        ];
        $context  = stream_context_create($options);
        $response = file_get_contents($url, false, $context);
        
        $result = json_decode($response);

        return $result->items;
    }
    public function convertFilial(object $filialFora): Filial
    {
        $arr = explode(",", $filialFora->address);
        return new Filial(
            null,
            2,
            $filialFora->id,
            $filialFora->city,
            $filialFora->city == "м. Київ" ? "Київська обл." : null,
            $arr[0],
            $arr[1],
            NumericHelper::toFloat($filialFora->lon),
            NumericHelper::toFloat($filialFora->lat),
            $filialFora->address
        );
    }
}
