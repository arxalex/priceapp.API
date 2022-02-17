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
        ];
    }
    public function build()
    {
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
