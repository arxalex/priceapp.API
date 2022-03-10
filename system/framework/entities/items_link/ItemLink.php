<?php

namespace framework\entities\items_link;

use framework\entities\default_entities\DefaultEntity;

class ItemLink extends DefaultEntity
{
    public ?int $id;
    public ?int $itemid;
    public ?int $shopid;
    public ?int $inshopid;
    public ?float $pricefactor;

    public function __construct(
        ?int $id = null,
        ?int $itemid = null,
        ?int $shopid = null,
        ?int $inshopid = null,
        ?float $pricefactor = null,
    ) {
        $this->id = $id;
        $this->itemid = $itemid;
        $this->shopid = $shopid;
        $this->inshopid = $inshopid;
        $this->pricefactor = $pricefactor;
    }
    
}
