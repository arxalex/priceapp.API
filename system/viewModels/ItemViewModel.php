<?php

namespace viewModels;

use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\items\Item;
use framework\entities\packages\PackagesService;

class ItemViewModel
{
    private CategoriesService $_categoriesService;
    private BrandsService $_brandsService;
    private PackagesService $_packagesService;

    public ?int $id;
    public ?string $label;
    public ?string $image;
    public ?int $category;
    public ?string $categoryLabel;
    public ?int $brand;
    public ?string $brandLabel;
    public ?int $package;
    public ?string $packageLabel;
    public ?string $packageUnits;
    public ?float $units;
    public ?float $term;
    public ?array $consist;
    public ?float $calorie;
    public ?float $carbohydrates;
    public ?float $fat;
    public ?float $proteins;
    public $additional;

    public function __construct(
        Item $item
    ) {
        $this->_categoriesService = new CategoriesService();
        $this->_brandsService = new BrandsService();
        $this->_packagesService = new PackagesService();
        $categoryLabel = $this->_categoriesService->getItemFromDB($item->category)->label;
        $brandLabel = $this->_brandsService->getItemFromDB($item->brand)->label;
        $package = $this->_packagesService->getItemFromDB($item->package);
        $packageLabel = $package->label;
        $packageUnits = $package->short;
        $this->id = $item->id;
        $this->label = $item->label;
        $this->image = $item->image;
        $this->category = $item->category;
        $this->categoryLabel = $categoryLabel;
        $this->brand = $item->brand;
        $this->brandLabel = $brandLabel;
        $this->package = $item->package;
        $this->packageLabel = $packageLabel;
        $this->packageUnits = $packageUnits;
        $this->units = $item->units;
        $this->term = $item->term;
        $this->consist = $item->consist;
        $this->calorie = $item->calorie;
        $this->carbohydrates = $item->carbohydrates;
        $this->fat = $item->fat;
        $this->proteins = $item->proteins;
        $this->additional = $item->additional;
    }
    
}
