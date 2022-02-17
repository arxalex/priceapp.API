<?php

namespace framework\shops\silpo;

use framework\shops\silpo\SilpoCategoriesModel;

class SilpoItemsGetter{
    public function __construct()
    {}
    public function get(int $fillialId = 2043): array
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'filialId' => $fillialId,
                'basketGuid' => null
            ],
            'method' => 'GetCategories'
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

        $categories = [];
        foreach($result->tree as $value) {
            $categories[] = new SilpoCategoriesModel(
                $value->id,
                $value->name
            );
        }
        return $categories;
    }
}
