<?php

namespace framework\entities\prices_history;

use framework\entities\default_entities\DefaultEntity;

class PriceHistory extends DefaultEntity
{
    public ?int $id;
    public ?int $itemid;
    public ?int $shopid;
    public ?float $price;
    public ?string $date;

    public function __construct(
        ?int $id = null,
        ?int $itemid = null,
        ?int $shopid = null,
        ?float $price = null,
        ?string $date = null
    ) {
        $this->id = $id;
        $this->itemid = $itemid;
        $this->shopid = $shopid;
        $this->price = $price;
        $this->date = $date;
    }
    
}
