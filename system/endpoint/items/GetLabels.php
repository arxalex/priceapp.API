<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\consists\ConsistsService;
use framework\entities\countries\CountriesService;
use framework\entities\packages\PackagesService;

class GetLabels extends BaseEndpointBuilder
{
    private CategoriesService $_categoriesService;
    private BrandsService $_brandService;
    private PackagesService $_packageService;
    private ConsistsService $_consistsService;
    private CountriesService $_countriesService;

    public function __construct()
    {
        parent::__construct();
        $this->_categoriesService = new CategoriesService();
        $this->_brandService = new BrandsService();
        $this->_packageService = new PackagesService();
        $this->_consistsService = new ConsistsService();
        $this->_countriesService = new CountriesService();
    }
    public function defaultParams()
    {
        return [
            'labelIds' => [],
            'method' => "GetForOneItem",
            'labelIdArrays' => null
        ];
    }
    public function build()
    {
        if ($this->getParam('method') == "GetForOneItem") {
            $labelIds = $this->getParam("labelIds");
            if (count($labelIds) < 1) {
                return [];
            }

            $response = [];
            foreach ($labelIds as $labelId) {
                $package = $labelId['packageId'] != null ? $this->_packageService->getItemFromDB($labelId['packageId']) : null;

                $response[] = new LabelsResponseViewModel(
                    $labelId['categoryId'] != null ? $this->_categoriesService->getItemFromDB($labelId['categoryId'])->label : null,
                    $labelId['brandId'] != null ? $this->_brandService->getItemFromDB($labelId['brandId'])->label : null,
                    $labelId['packageId'] != null ? $package->label : null,
                    count($labelId['consistIds']) < 1 ? $this->_consistsService->getColumn($this->_consistsService->getItemsFromDB([
                        "id" => $labelId['consistIds']
                    ]), "label") : [],
                    $labelId['countryId'] != null ? $this->_countriesService->getItemFromDB($labelId['countryId'])->label : null,
                    $labelId['packageId'] != null ? $package->short : null
                );
            }
            return $response;
        } elseif ($this->getParam('method') == "GetForMultipleItems"){
            $labelIds = $this->getParam('labelIdArrays');
            $categories = count($labelIds->categoryIds) > 0 ? $this->_categoriesService->getItemsFromDB([
                "id" => $labelIds->categoryIds
            ]) : [];
            $brands = count($labelIds->brandIds) ? $this->_brandService->getItemsFromDB([
                "id" => $labelIds->brandIds
            ]) : [];
            $packages = count($labelIds->packageIds) ? $this->_packageService->getItemsFromDB([
                "id" => $labelIds->packageIds
            ]) : [];
            $consists = count($labelIds->consistIds) ? $this->_consistsService->getItemsFromDB([
                "id" => $labelIds->consistIds
            ]) : [];
            $countries = count($labelIds->countryIds) ? $this->_countriesService->getItemsFromDB([
                "id" => $labelIds->countryIds
            ]) : [];
            $response = new LabelArraysResponseViewModel(
                $this->_categoriesService->getColumns($categories, ["id", "label"]),
                $this->_brandService->getColumns($brands, ["id", "label"]),
                $this->_packageService->getColumns($packages, ["id", "label", "short"]),
                $this->_consistsService->getColumns($consists, ["id", "label"]),
                $this->_countriesService->getColumns($countries, ["id", "label"])
            );
            return $response;
        } elseif($this->getParam('method') == "GetAllLabels") {
            $categories = $this->_categoriesService->getItemsFromDB();
            $brands = $this->_brandService->getItemsFromDB();
            $packages = $this->_packageService->getItemsFromDB();
            $consists = $this->_consistsService->getItemsFromDB();
            $countries = $this->_countriesService->getItemsFromDB();
            $response = new LabelArraysResponseViewModel(
                $this->_categoriesService->getColumns($categories, ["id", "label"]),
                $this->_brandService->getColumns($brands, ["id", "label"]),
                $this->_packageService->getColumns($packages, ["id", "label", "short"]),
                $this->_consistsService->getColumns($consists, ["id", "label"]),
                $this->_countriesService->getColumns($countries, ["id", "label"])
            );
            return $response;
        }
    }
}
class LabelsResponseViewModel
{
    public ?string $categoryLabel;
    public ?string $brandLabel;
    public ?string $packageLabel;
    public array $consistLabels;
    public ?string $countryLabel;
    public ?string $packageShort;

    public function __construct(
        ?string $categoryLabel = null,
        ?string $brandLabel = null,
        ?string $packageLabel = null,
        array $consistLabels = [],
        ?string $countryLabel = null,
        ?string $packageShort = null
    ) {
        $this->categoryLabel = $categoryLabel;
        $this->brandLabel = $brandLabel;
        $this->packageLabel = $packageLabel;
        $this->consistLabels = $consistLabels;
        $this->countryLabel = $countryLabel;
        $this->packageShort = $packageShort;
    }
}
class LabelArraysResponseViewModel
{
    public array $categories;
    public array $brands;
    public array $packages;
    public array $consists;
    public array $countries;

    public function __construct(
        array $categories = [],
        array $brands = [],
        array $packages = [],
        array $consists = [],
        array $countries = []
    ) {
        $this->categories = $categories;
        $this->brands = $brands;
        $this->packages = $packages;
        $this->consists = $consists;
        $this->countries = $countries;
    }
}