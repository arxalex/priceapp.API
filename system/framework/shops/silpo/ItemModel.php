<?php

namespace framework\shops\silpo;

class ItemModel{
    public string $label;
    public string $image;
    public int $category;
    public string $brand;
    public string $packege;
    public int $units;
    public int $term;
    public ?array $barcodes;
    public ?array $consist;
    public ?float $calorie;
    public ?float $carbohydrates;
    public ?float $fat;
    public ?float $proteins;
    public $additional;

    public function __construct(
        ?int $id = null,
        ?string $label = null,
        ?string $image = null,
        ?int $category = null,
        ?int $brand = null,
        ?int $packege = null,
        ?int $units = null,
        ?int $term = null,
        ?string $barcodes = null,
        ?string $consist = null,
        ?float $calorie = null,
        ?float $carbohydrates = null,
        ?float $fat = null,
        ?float $proteins = null,
        $additional = null
    ) {
        $this->id = $id;
        $this->label = $label;
        $this->image = $image;
        $this->category = $category;
        $this->brand = $brand;
        $this->packege = $packege;
        $this->units = $units;
        $this->term = $term;
        $this->barcodes = $this->stringConvert($barcodes);
        $this->consist = $this->stringConvert($consist);
        $this->calorie = $calorie;
        $this->carbohydrates = $carbohydrates;
        $this->fat = $fat;
        $this->proteins = $proteins;
        $this->additional = json_decode($additional);
    }
}