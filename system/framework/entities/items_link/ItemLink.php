<?php

namespace framework\entities\items_link;

class ItemLink
{
    public ?int $id;
    public ?int $itemid;
    public ?int $shopid;
    public ?int $inshopid;

    public function __construct(
        ?int $id = null,
        ?int $itemid = null,
        ?int $shopid = null,
        ?int $inshopid = null
    ) {
        $this->id = $id;
        $this->itemid = $itemid;
        $this->shopid = $shopid;
        $this->inshopid = $inshopid;
    }
    
}
