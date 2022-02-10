<?php

namespace framework\entities\prices_history;

use DateTime;

class PriceHistory
{
    public ?int $id;
    public ?int $itemid;
    public ?int $shopid;
    public ?float $price;
    public ?DateTime $date;

    public function __construct(
        ?int $id = null,
        ?int $itemid = null,
        ?int $shopid = null,
        ?float $price = null,
        ?DateTime $date = null
    ) {
        $this->id = $id;
        $this->itemid = $itemid;
        $this->shopid = $shopid;
        $this->price = $price;
        $this->date = $date;
    }
    
}
