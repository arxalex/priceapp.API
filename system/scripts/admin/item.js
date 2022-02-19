Vue.component('item', {
    template: `
        <div class="row">
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ item.label }}</h1>
                    Category: {{ item.category }} - {{ itemLabels.categoryLabel }}<br>
                    Brand: {{ item.brand }} - {{ itemLabels.brandLabel }}<br>
                    Package: {{ item.package }} - {{ itemLabels.packageLabel }}, units: {{ item.units }} {{ itemLabels.packageShort }}, term: {{ item.term }}<br>
                    Barcodes: <span v-for="barcode in item.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in item.consist">{{ consistId }}, </span> - <span v-for="consistLabel in itemLabels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ item.calorie }}, carbohydrates: {{ item.carbohydrates }}, fat: {{ item.fat }}, proteins: {{ item.proteins }}<br>
                    Country: {{ item.additional.country }} - {{ itemLabels.countryLabel }} <br>
                </div>
            </div>
            <div class="col-sm-6 row">
                <div class="col-sm-4" v-if="currentSimilarItem !== null">
                    <img class="w-100" :src="currentSimilarItem.image">
                </div>
                <div class="col-sm-8" v-if="currentSimilarItem !== null">
                    <h1>{{ currentSimilarItem.label }}</h1>
                    Category: {{ currentSimilarItem.category }} - {{ currentSimilarLabels.categoryLabel }}<br>
                    Brand: {{ currentSimilarItem.brand }} - {{ currentSimilarLabels.brandLabel }}<br>
                    Package: {{ currentSimilarItem.package }} - {{ currentSimilarLabels.packageLabel }}, units: {{ currentSimilarItem.units }} {{ currentSimilarLabels.packageShort }}, term: {{ currentSimilarItem.term }}<br>
                    Barcodes: <span v-for="barcode in currentSimilarItem.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in currentSimilarItem.consist">{{ consistId }}, </span> - <span v-for="consistLabel in currentSimilarLabels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ currentSimilarItem.calorie }}, carbohydrates: {{ currentSimilarItem.carbohydrates }}, fat: {{ currentSimilarItem.fat }}, proteins: {{ currentSimilarItem.proteins }}<br>
                    Country: {{ currentSimilarItem.additional.country }} - {{ currentSimilarLabels.countryLabel }}<br>
                </div>
                <div class="">
                    <select class="form-select" v-model="currentSimilarId">
                        <option selected disabled>Chose opinion</option>
                        <option name="item" value="0">New item</option>
                        <option v-for="similarOption in similarOptions" v-bind:value="similarOption.value">{{ similarOption.text }}</option>
                    </select>
                    <button class="btn btn-primary" @click="insert">Insert</button>
                </div>
            </div>
            <itemsaver :sourceItem="item"
                :destinationItem="currentSimilarItem"
                v-if="saveActive"></itemsaver>
        </div>
    `,
    data() {
        return {
            currentSimilarId: 0,
            saveActive: false,
        }
    },
    methods: {
        insert: function () {
            this.saveActive = true;
        }
    },
    props: {
        item: {
            type: Object
        },
        similarItems: {
            type: Array
        },
    },
    mounted(){
        this.item.consist = this.item.consist ?? [];
    },
    computed: {
        currentSimilarItem: function () {
            if (this.currentSimilarId == 0) {
                var item = this.item;
                item.consist = item.consist ?? [];
                return item;
            } else {
                var item = this.similarItems[this.currentSimilarId - 1];
                item.consist = item.consist ?? [];
                return item;
            }
        },
        currentSimilarLabels: function () {
            if (this.currentSimilarId == 0) {
                return this.itemLabels;
            } else {
                return this.similarLabels[this.currentSimilarId - 1];
            }
        },
        itemLabels: function () {
            var itemCategoryLabels = this.$labels.categories.filter(value => value.id == this.item.category);
            var itemBrandLabels = this.$labels.brands.filter(value => value.id == this.item.brand);
            var itemPackageLabels = this.$labels.packages.filter(value => value.id == this.item.package);
            var itemConsistLabels = this.item.consist !== null && this.item.consist.length > 0 ? this.$labels.consists.filter(value => this.item.consist.includes(value.id)) : [];
            var itemCountryLabels = this.$labels.countries.filter(value => value.id == this.item.additional.country);
            return {
                categoryLabel: itemCategoryLabels.length > 0 ? itemCategoryLabels[0].label : null,
                brandLabel: itemBrandLabels.length > 0 ? itemBrandLabels[0].label : null,
                packageLabel: itemPackageLabels.length > 0 ? itemPackageLabels[0].label : null,
                packageShort: itemPackageLabels.length > 0 ? itemPackageLabels[0].short : null,
                consistLabels: itemConsistLabels.length > 0 ? itemConsistLabels.map(value => value.label) : [],
                countryLabel: itemCountryLabels.length > 0 ? itemCountryLabels[0].label : null
            };
        },
        similarLabels: function () {
            return this.similarItems.map(similarItem => {
                var categoryLabels = this.$labels.categories.filter(value => value.id == similarItem.category);
                var brandLabels = this.$labels.brands.filter(value => value.id == similarItem.brand);
                var packageLabels = this.$labels.packages.filter(value => value.id == similarItem.package);
                var consistLabels = similarItem.consist !== null && similarItem.consist.length > 0 ? this.$labels.consists.filter(value => similarItem.consist.includes(value.id)) : [];
                var countryLabels = this.$labels.countries.filter(value => value.id == similarItem.additional.country);
                return {
                    categoryLabel: categoryLabels.length > 0 ? categoryLabels[0].label : null,
                    brandLabel: brandLabels.length > 0 ? brandLabels[0].label : null,
                    packageLabel: packageLabels.length > 0 ? packageLabels[0].label : null,
                    packageShort: packageLabels.length > 0 ? packageLabels[0].short : null,
                    consistLabels: consistLabels.length > 0 ? consistLabels.map(value => value.label) : [],
                    countryLabel: countryLabels.length > 0 ? countryLabels[0].label : null
                };
            });
        },
        similarOptions: function () {
            var array = [];
            var i = 1;
            this.similarItems.forEach(element => {
                array.push({ text: element.label, value: i++ });
            });
            return array;
        }
    }
});