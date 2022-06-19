<?php

namespace framework\entities\prices;

use framework\entities\default_entities\DefaultEntity;

class Price extends DefaultEntity
{
    public ?int $id;
    public ?int $itemid;
    public ?int $shopid;
    public ?float $price;
    public ?int $filialid;
    public ?float $quantity;

    public function __construct(
        ?int $id = null,
        ?int $itemid = null,
        ?int $shopid = null,
        ?float $price = null,
        ?int $filialid = null,
        ?float $quantity = 0
    ) {
        $this->id = $id;
        $this->itemid = $itemid;
        $this->shopid = $shopid;
        $this->price = $price;
        $this->filialid = $filialid;
        $this->quantity = $quantity;
    }
    
}
