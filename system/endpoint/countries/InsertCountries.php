<?php

namespace endpoint\countries;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\countries\CountriesService;
use framework\entities\countries\Country;
use stdClass;

class InsertCountries extends BaseEndpointBuilder
{
    private CountriesService $_countriesService;
    public function __construct()
    {
        $this->_countriesService = new CountriesService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'country' => null,
            'method' => "InsertOrUpdateCountry"
        ];
    }
    public function build()
    {
        if ($this->getParam('method') == "InsertOrUpdateCountry") {
            $countryModel = (object) $this->getParam('country');
            $result = new stdClass();
            if ($countryModel->id == null || $countryModel->id == "") {
                $country = new Country(null, $countryModel->label, $countryModel->short);
                $this->_countriesService->insertItemToDB($country);
                $country = $this->_countriesService->getLastInsertedItem();
                $statusInsert = $this->_countriesService->getItemFromDB($country->id)->label == $countryModel->label;
                $result->statusInsert = $statusInsert;
                $result->country = $country;
            } else {
                $country = new Country(intval($countryModel->id), $countryModel->label, $countryModel->short);
                $this->_countriesService->updateItemInDB($country);
                $statusUpdate = $this->_countriesService->getItemFromDB($country->id)->label == $country->label;
                $result->statusUpdate = $statusUpdate;
            }

            return $result;
        }
    }
}
