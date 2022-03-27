<?php

namespace framework\entities\filials;

use framework\entities\default_entities\DefaultEntity;

class Filial extends DefaultEntity
{
    public ?int $id;
    public ?int $shopid;
    public ?int $inshopid;
    public ?string $city;
    public ?string $region;
    public ?string $street;
    public ?string $house;
    public ?float $xcord;
    public ?float $ycord;
    public ?string $label;

    public function __construct(
        ?int $id = null,
        ?int $shopid = null,
        ?int $inshopid = null,
        ?string $city = null,
        ?string $region = null,
        ?string $street = null,
        ?string $house = null,
        ?float $xcord = null,
        ?float $ycord = null,
        ?string $label = null
    ) {
        $this->id = $id;
        $this->shopid = $shopid;
        $this->inshopid = $inshopid;
        $this->city = $city;
        $this->region = $region;
        $this->street = $street;
        $this->house = $house;
        $this->xcord = $xcord;
        $this->ycord = $ycord;
        $this->label = $label;
    }
    
}
