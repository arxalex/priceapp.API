<?php

namespace framework\entities_proxy\items;

use framework\entities\default_entities\DefaultEntity;

class AtbItem extends DefaultEntity
{
    public ?int $id;
    public ?int $internalid;
    public ?string $label;
    public ?string $image;
    public ?int $category;
    public ?string $brand;
    public ?string $country;

    public function __construct(
        ?int $id = null,
        ?int $internalid = null,
        ?string $label = null,
        ?string $image = null,
        ?int $category = null,
        ?string $brand = null,
        ?string $country = null
    ) {
        $this->id = $id;
        $this->internalid = $internalid;
        $this->label = $label;
        $this->image = $image;
        $this->category = $category;
        $this->brand = $brand;
        $this->country = $country;
    }
    
}
