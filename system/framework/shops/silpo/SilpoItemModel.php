<?php

namespace framework\shops\silpo;

class SilpoItemModel{
    public ?int $inshopid;
    public ?string $label;
    public ?string $image;
    public ?int $shopcategoryid;
    public ?string $brand;
    public ?string $package;
    public ?float $alcohol;
    public ?float $units;
    public ?float $calorie;
    public ?float $carbohydrates;
    public ?float $fat;
    public ?float $proteins;
    public ?string $country;
    public ?string $url;

    public function __construct(
        ?int $inshopid = null,
        ?string $label = null,
        ?string $image = null,
        ?int $shopcategoryid = null,
        ?string $brand = null,
        ?string $package = null,
        ?float $alcohol = null,
        ?float $units = null,
        ?float $calorie = null,
        ?float $carbohydrates = null,
        ?float $fat = null,
        ?float $proteins = null,
        ?string $country = null,
        ?string $url
    ) {
        $this->inshopid = $inshopid;
        $this->label = $label;
        $this->image = $image;
        $this->shopcategoryid = $shopcategoryid;
        $this->brand = $brand;
        $this->package = $package;
        $this->alcohol = $alcohol;
        $this->units = $units;
        $this->calorie = $calorie;
        $this->carbohydrates = $carbohydrates;
        $this->fat = $fat;
        $this->proteins = $proteins;
        $this->country = $country;
        $this->url = $url;
    }
}