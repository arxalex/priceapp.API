<?php

namespace framework\entities\categories_link;

use framework\entities\default_entities\DefaultEntity;

class CategoryLink extends DefaultEntity
{
    public ?int $id;
    public ?int $categoryid;
    public ?int $shopid;
    public ?int $categoryshopid;
    public ?string $shopcategorylabel;

    public function __construct(
        ?int $id = null,
        ?int $categoryid = null,
        ?int $shopid = null,
        ?int $categoryshopid = null,
        ?string $shopcategorylabel = null
    ) {
        $this->id = $id;
        $this->categoryid = $categoryid;
        $this->shopid = $shopid;
        $this->categoryshopid = $categoryshopid;
        $this->shopcategorylabel = $shopcategorylabel;
    }
    
}
