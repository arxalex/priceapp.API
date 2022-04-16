<?php

namespace framework\entities\shops;

use framework\entities\default_entities\DefaultEntity;

class Shop extends DefaultEntity
{
    public ?int $id;
    public ?string $label;
    public ?int $country;
    public ?string $icon;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
        ?int $country = null,
        ?string $icon = null
    ) {
        $this->id = $id;
        $this->label = $label;
        $this->country = $country;
        $this->icon = $icon;
    }
    
}
