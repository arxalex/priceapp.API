<?php

namespace endpoint\packages;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\packages\PackagesService;
use framework\entities\packages\Package;
use stdClass;

class InsertPackages extends BaseEndpointBuilder
{
    private PackagesService $_packagesService;
    public function __construct()
    {
        $this->_packagesService = new PackagesService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'package' => null,
            'method' => "InsertOrUpdatePackage",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "InsertOrUpdatePackage") {
            $packageModel = (object) $this->getParam('package');
            $result = new stdClass();
            if ($packageModel->id == null || $packageModel->id == "") {
                $package = new Package(null, $packageModel->label, $packageModel->short);
                $this->_packagesService->insertItemToDB($package);
                $package = $this->_packagesService->getLastInsertedItem();
                $statusInsert = $this->_packagesService->getItemFromDB($package->id)->label == $packageModel->label;
                $result->statusInsert = $statusInsert;
                $result->package = $package;
            } else {
                $package = new Package(intval($packageModel->id), $packageModel->label, $packageModel->short);
                $this->_packagesService->updateItemInDB($package);
                $statusUpdate = $this->_packagesService->getItemFromDB($package->id)->label == $package->label;
                $result->statusUpdate = $statusUpdate;
            }

            return $result;
        }
    }
}
