<?php

namespace framework\shops\atb;

class AtbItemModel{
    public ?int $inshopid;
    public ?string $label;
    public ?string $image;
    public ?int $shopcategoryid;
    public ?string $brand;
    public ?string $country;
    public ?string $url;

    public function __construct(
        ?int $inshopid = null,
        ?string $label = null,
        ?string $image = null,
        ?int $shopcategoryid = null,
        ?string $brand = null,
        ?string $country = null,
        ?string $url
    ) {
        $this->inshopid = $inshopid;
        $this->label = $label;
        $this->image = $image;
        $this->shopcategoryid = $shopcategoryid;
        $this->brand = $brand;
        $this->country = $country;
        $this->url = $url;
    }
}