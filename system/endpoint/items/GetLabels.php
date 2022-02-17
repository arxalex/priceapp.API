<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\consists\ConsistsService;
use framework\entities\packages\PackagesService;

class GetLabels extends BaseEndpointBuilder
{
    private CategoriesService $_categoriesService;
    private BrandsService $_brandService;
    private PackagesService $_packageService;
    private ConsistsService $_consistsService;
    public function __construct()
    {
        parent::__construct();
        $this->_categoriesService = new CategoriesService();
        $this->_brandService = new BrandsService();
        $this->_packageService = new PackagesService();
        $this->_consistsService = new ConsistsService();
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
            $response[] = new LabelsResponseViewModel(
                $labelId['categoryId'] != null ? $this->_categoriesService->getItemFromDB($labelId['categoryId'])->label : null,
                $labelId['brandId'] != null ? $this->_brandService->getItemFromDB($labelId['brandId'])->label : null,
                $labelId['packageId'] != null ? $this->_packageService->getItemFromDB($labelId['packageId'])->label : null,
                count($labelId['consistIds']) < 1 ? $this->_consistsService->getColumn($this->_consistsService->getItemsFromDB([
                    "id" => $labelId['consistIds']
                ]), "label") : []
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
    public function __construct(
        ?string $categoryLabel = null,
        ?string $brandLabel = null,
        ?string $packageLabel = null,
        array $consistLabels = []
    ) {
        $this->categoryLabel = $categoryLabel;
        $this->brandLabel = $brandLabel;
        $this->packageLabel = $packageLabel;
        $this->consistLabels = $consistLabels;
    }
}
