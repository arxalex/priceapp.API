Vue.component('itemsaver', {
    template: `
        <div>
            <categoryInsert 
                v-if="isCategoryInsertAwailible"
                @categoryInserted="itemInserted('category')"
                :sourceCategory="destinationSelect.category">
            </categoryInsert>
            <packageInsert 
                v-if="isPackageInsertAwailible"
                @packageInserted="itemInserted('package')"
                :sourcePackage="destinationSelect.package">
            </packageInsert>
            <brandInsert 
                v-if="isBrandInsertAwailible"
                @brandInserted="itemInserted('brand')"
                :sourceBrand="destinationSelect.brand">
            </brandInsert>
            <table class="table word-break">
                <tbody>
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
                            <a class="btn btn-primary">
                                <i class="bi bi-plus-square"></i>
                            </a>
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
                </tbody>
            </table>
            <button class="btn btn-primary" v-on:click='$emit("insert")'></button>
        </div>
    `,
    data() {
        return {
            isCategoryInsertAwailible: false,
            isPackageInsertAwailible: false,
            isBrandInsertAwailible: false
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
        insertItem: function (source) {
            switch (source) {
                case "category":
                    this.isCategoryInsertAwailible = true;
                    break;
                case "package":
                    this.isPackageInsertAwailible = true;
                    break;
                case "brand":
                    this.isBrandInsertAwailible = true;
                    break;
            }
        },
        itemInserted: function (source) {
            switch (source) {
                case "category":
                    this.isCategoryInsertAwailible = false;
                    break;
                case "package":
                    this.isPackageInsertAwailible = false;
                    break;
                case "brand":
                    this.isBrandInsertAwailible = false;
                    break;
            }
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
            var itemConsistLabels = this.destinationItem.consist.length != 1 || this.destinationItem.consist[0] != -1 ?
                (this.sourceItem.consist !== null && this.destinationItem.consist.length > 0 ?
                    this.$labels.consists.filter(value => this.destinationItem.consist.includes(value.id)) :
                    []) :
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
        consists: {
            cache: false,
            get: function () {
                return this.$labels.consists;
            }
        }
    }
});

