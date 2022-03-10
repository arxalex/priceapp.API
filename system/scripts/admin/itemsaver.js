Vue.component('itemsaver', {
    template: `
        <div>
            <categoryInsert 
                v-if="isInsertAwailible.category"
                @itemInserted="itemInserted('category')"
                :sourceItem="destinationSelect.category">
            </categoryInsert>
            <packageInsert 
                v-if="isInsertAwailible.package"
                @itemInserted="itemInserted('package')"
                :sourceItem="destinationSelect.package">
            </packageInsert>
            <brandInsert 
                v-if="isInsertAwailible.brand"
                @itemInserted="itemInserted('brand')"
                :sourceItem="destinationSelect.brand">
            </brandInsert>
            <consistInsert 
                v-if="isInsertAwailible.consist"
                @itemInserted="itemInserted('consist')"
                :sourceItem="destinationSelect.consist">
            </consistInsert>
            <countryInsert 
                v-if="isInsertAwailible.country"
                @itemInserted="itemInserted('country')"
                :sourceItem="destinationSelect.country">
            </countryInsert>
            <div class="d-flex mb-3 p-1">
                <h1 class="ms-1 fw-bold flex-fill">Item</h1>
                <button class="btn mt-0 pt-0 px-0" @click="$emit('insertCanceled')">
                    <i class="bi bi-x text-danger"></i>
                </button>
            </div>
            <table class="table word-break">
                <tbody>
                    <tr>
                        <th>
                            <span>Id: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.id }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.id">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Image: </span>
                        </th>
                        <td>
                            <img :src="sourceItem.image">
                        </td>
                        <td>
                            <img :src="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Image url: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.image }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Label: </span>
                        </th>
                        <td>
                            <span><a :href="originalLabels.url">{{ sourceItem.label }}</a></span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.label">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Category: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.categoryLabel }} ({{ sourceItem.category }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.categoryLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.category">
                                <option disabled>Chose category</option>
                                <option :value="-1">New item</option>
                                <option v-for="category in categories" v-bind:value="category.id">{{ category.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertItem('category')">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Package: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.packageLabel }} ({{ sourceItem.package }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.packageLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.package">
                                <option disabled>Chose package</option>
                                <option :value="-1">New item</option>
                                <option v-for="package in packages" v-bind:value="package.id">{{ package.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertItem('package')">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Brand: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.brandLabel }} ({{ sourceItem.brand }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.brandLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.brand">
                                <option disabled>Chose brand</option>
                                <option :value="-1">New item</option>
                                <option v-for="brand in brands" :value="brand.id">{{ brand.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertItem('brand')">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Consist: </span>
                        </th>
                        <td>
                            <span v-for="consistLabel in sourceLabels.consistLabels">{{ consistLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" multiple v-model="destinationItem.consist">
                                <option disabled>Chose consists</option>
                                <option v-for="consist in consists" v-bind:value="consist.id">{{ consist.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertItem('consist')">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Units: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.units }} {{ sourceLabels.packageShort }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.units">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Term: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.term }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.term">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Calorie: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.calorie }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.calorie">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Carbohydrates: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.carbohydrates }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.carbohydrates">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Fat: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.fat }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.fat">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Proteins: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.proteins }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.proteins">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Country: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.countryLabel }} ({{ sourceItem.additional.country }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.countryLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.additional.country">
                                <option disabled>Chose country</option>
                                <option :value="-1">New item</option>
                                <option v-for="country in countries" :value="country.id">{{ country.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertItem('country')">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Price factor (Dest. price / source price):</span>
                        </th>
                        <td colspan="2">
                            <input class="form-control" v-model="itemLink.pricefactor">
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="input-group mb-3">
                <button class="btn btn-primary w-100" v-on:click='insert(1)'>Insert and Link</button>
                <button class="btn btn-primary w-100" v-on:click='insert(2)'>Link</button>
            </div>
        </div>
    `,
    data() {
        return {
            isInsertAwailible: {
                category: false,
                package: false,
                brand: false,
                consist: false,
                country: false
            },
            itemLink: {
                id: null,
                itemid: null,
                shopid: null,
                inshopid: null,
                pricefactor: 1
            }
        }
    },
    props: {
        sourceItem: {
            type: Object
        },
        destinationItem: {
            type: Object
        },
        originalLabels: {
            type: Object
        }
    },
    methods: {
        insert: async function(method){
            switch(method){
                case 1:
                    const insertUrl = "../be/items/insert_items";
                    var data = {
                        method: "InsertOrUpdateItem",
                        item: this.destinationItem,
                        item_link: this.itemLink
                    }
                    await this.getItemsFromDb(insertUrl, data);
                    this.$emit("insert");
                    break;
                case 2:
                    const insertUrl = "../be/items/insert_items";
                    var data = {
                        method: "LinkItem",
                        item: this.destinationItem,
                        item_link: this.itemLink
                    }
                    await this.getItemsFromDb(insertUrl, data);
                    this.$emit("insert");
                    break;
            }
        },
        insertItem: function (source) {
            switch (source) {
                case "category":
                    this.isInsertAwailible.category = true;
                    break;
                case "package":
                    this.isInsertAwailible.package = true;
                    break;
                case "brand":
                    this.isInsertAwailible.brand = true;
                    break;
                case "consist":
                    this.isInsertAwailible.consist = true;
                    break;
                case "country":
                    this.isInsertAwailible.country = true;
                    break;
            }
        },
        itemInserted: function (source) {
            switch (source) {
                case "category":
                    this.isInsertAwailible.category = false;
                    break;
                case "package":
                    this.isInsertAwailible.package = false;
                    break;
                case "brand":
                    this.isInsertAwailible.brand = false;
                    break;
                case "consist":
                    this.isInsertAwailible.consist = false;
                    break;
                case "country":
                    this.isInsertAwailible.country = false;
                    break;
            }
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        }
    },
    computed: {
        sourceLabels: function () {
            var itemCategoryLabels = this.$labels.categories.filter(value => value.id == this.sourceItem.category);
            var itemBrandLabels = this.$labels.brands.filter(value => value.id == this.sourceItem.brand);
            var itemPackageLabels = this.$labels.packages.filter(value => value.id == this.sourceItem.package);
            var itemConsistLabels = this.sourceItem.consist !== null && this.sourceItem.consist.length > 0 ? this.$labels.consists.filter(value => this.sourceItem.consist.includes(value.id)) : [];
            var itemCountryLabels = this.$labels.countries.filter(value => value.id == this.sourceItem.additional.country);

            return {
                categoryLabel: itemCategoryLabels.length > 0 ? itemCategoryLabels[0].label : null,
                brandLabel: itemBrandLabels.length > 0 ? itemBrandLabels[0].label : null,
                packageLabel: itemPackageLabels.length > 0 ? itemPackageLabels[0].label : null,
                consistLabels: itemConsistLabels.length > 0 ? itemConsistLabels.map(value => value.label) : [],
                countryLabel: itemCountryLabels.length > 0 ? itemCountryLabels[0].label : null,
                packageShort: itemPackageLabels.length > 0 ? itemPackageLabels[0].short : null,
            };
        },
        destinationSelect: function () {

            var itemCategoryLabels = this.destinationItem.category != -1 ?
                this.$labels.categories.filter(value => value.id == this.destinationItem.category) :
                [{
                    id: null,
                    label: this.originalLabels.categoryLabel ?? "",
                    parent: null,
                    isFilter: false
                }];
            var itemBrandLabels = this.destinationItem.brand != -1 ?
                this.$labels.brands.filter(value => value.id == this.destinationItem.brand) :
                [{
                    id: null,
                    label: this.originalLabels.brandLabel ?? "",
                    short: ""
                }];
            var itemPackageLabels = this.destinationItem.package != -1 ?
                this.$labels.packages.filter(value => value.id == this.destinationItem.package) :
                [{
                    id: null,
                    label: this.originalLabels.packageLabel ?? "",
                    short: ""
                }];
            var itemConsistLabels = this.sourceItem.consist !== null && this.destinationItem.consist.length > 0 ?
                this.$labels.consists.filter(value => this.destinationItem.consist.includes(value.id)) :
                [{
                    id: null,
                    label: "",
                }];
            var itemCountryLabels = this.destinationItem.additional.country != -1 ?
                this.$labels.countries.filter(value => value.id == this.destinationItem.additional.country) :
                [{
                    id: null,
                    label: "",
                    short: ""
                }];

            return {
                category: itemCategoryLabels.length > 0 ? itemCategoryLabels[0] : null,
                package: itemPackageLabels.length > 0 ? itemPackageLabels[0] : null,
                brand: itemBrandLabels.length > 0 ? itemBrandLabels[0] : null,
                consist: itemConsistLabels.length > 0 ? itemConsistLabels[0] : null,
                country: itemCountryLabels.length > 0 ? itemCountryLabels[0] : null,
            }
        },
        categories: {
            cache: false,
            get: function () {
                return this.$labels.categories;
            }
        },
        packages: {
            cache: false,
            get: function () {
                return this.$labels.packages;
            }
        },
        brands: {
            cache: false,
            get: function () {
                return this.$labels.brands;
            }
        },
        countries: {
            cache: false,
            get: function () {
                return this.$labels.countries;
            }
        },
        consists: {
            cache: false,
            get: function () {
                return this.$labels.consists;
            }
        }
    }
});

